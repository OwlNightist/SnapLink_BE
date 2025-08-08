using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SnapLink_Model.DTO;
using SnapLink_Repository.DBContext;
using SnapLink_Service.IService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SnapLink_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly SnaplinkDbContext _context;
        private readonly IAuthService _auth;
       

        public AuthController(IOptions<JwtSettings> jwtOptions, SnaplinkDbContext context, IAuthService auth)
        {
            _jwtSettings = jwtOptions.Value;
            _context = context;
            _auth = auth;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.Email == dto.Email && u.PasswordHash == dto.Password);

            if (user == null) return Unauthorized("Invalid credentials");
            if (user.IsVerified != true) 
                return Unauthorized("Account not verified. Please check your email.");


            var roles = _context.UserRoles
                .Where(ur => ur.UserId == user.UserId)
                .Select(ur => ur.Role.RoleName)
                .ToList();

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.FullName ?? user.Email)
        };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
                signingCredentials: creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expires = token.ValidTo
            });
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logged out successfully." });
        }

        [HttpPost("forgot-password/start")]
        public async Task<IActionResult> ForgotPasswordStart([FromBody] ForgotPasswordRequest req)
        {
            try { return Ok(new { message = await _auth.ForgotPasswordStartAsync(req) }); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("forgot-password/verify")]
        public async Task<IActionResult> VerifyResetCode([FromBody] VerifyResetCodeRequest req)
        {
            try { return Ok(new { message = await _auth.VerifyResetCodeAsync(req) }); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("forgot-password/reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest req)
        {
            try { return Ok(new { message = await _auth.ResetPasswordAsync(req) }); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
