using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO;
using SnapLink_Service.IService;
using System.Security.Claims;

namespace SnapLink_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateUserDto dto)
        {
            var result = await _userService.CreateUserWithRoleAsync(dto, "Admin");
            return Ok(result);
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var result = await _userService.CreateUserWithRoleAsync(dto, "User");
            return Ok(result);
        }

        [HttpPost("create-photographer")]
        public async Task<IActionResult> CreatePhotographer([FromBody] CreateUserDto dto)
        {
            var result = await _userService.CreateUserWithRoleAsync(dto, "Photographer");
            return Ok(result);
        }

        [HttpPost("create-locationowner")]
        public async Task<IActionResult> CreateLocationOwner([FromBody] CreateUserDto dto)
        {
            var result = await _userService.CreateUserWithRoleAsync(dto, "LocationOwner");
            return Ok(result);
        }

        [HttpPost("create-moderator")]
        public async Task<IActionResult> CreateModerator([FromBody] CreateUserDto dto)
        {
            var result = await _userService.CreateUserWithRoleAsync(dto, "Moderator");
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto dto)
        {
            var result = await _userService.UpdateUserAsync(dto);
            return Ok(result);
        }

        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var result = await _userService.DeleteUserAsync(userId);
            return Ok(result);
        }

        [HttpDelete("hard-delete/{userId}")]
        public async Task<IActionResult> HardDeleteAccount(int userId)
        {
            var result = await _userService.HardDeleteUserAsync(userId);
            if (result.Contains("deleted"))
                return Ok(result);
            return NotFound(result);
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        [HttpGet("GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null) return NotFound($"User with email '{email}' not found");
            return Ok(user);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null) return NotFound("User not found.");
            return Ok(user);
        }

        [HttpGet("by-role/{roleName}")]
        public async Task<IActionResult> GetUsersByRole(string roleName)
        {
            var users = await _userService.GetUsersByRoleNameAsync(roleName);
            if (users == null || !users.Any())
                return NotFound("No users found for the given role.");
            return Ok(users);
        }
        [HttpPost("assign-roles")]
        public async Task<IActionResult> AssignRolesToUser([FromBody] AssignRolesDto request)
        {
            var result = await _userService.AssignRolesToUserAsync(request);
            if (result)
                return Ok("Roles assigned successfully.");
            return BadRequest("Failed to assign roles.");
        }
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto)
        {
            var success = await _userService.VerifyEmailAsync(dto.Email, dto.Code);
            if (!success) return BadRequest("Invalid or expired verification code.");
            return Ok("Email verified successfully.");
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("Không xác định được user.");
            if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized("UserId không hợp lệ.");

            try
            {
                var msg = await _userService.ChangePasswordAsync(userId, dto);
                return Ok(new { message = msg });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("change-password-for/{targetUserId:int}")]
        public async Task<IActionResult> ChangePasswordFor(int targetUserId, [FromBody] ChangePasswordDto dto)
        {
            try
            {
                var msg = await _userService.ChangePasswordAsync(targetUserId, dto);
                return Ok(new { message = msg });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
