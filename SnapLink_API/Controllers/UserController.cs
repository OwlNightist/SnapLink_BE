using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO;
using SnapLink_Service.IService;

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
    }
}
