using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO;
using SnapLink_Model.DTO.Response;
using SnapLink_Service.IService;
using AutoMapper;

namespace SnapLink_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
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
            var userResponses = _mapper.Map<List<UserResponse>>(users);
            return Ok(userResponses);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null) return NotFound("User not found.");
            
            var userResponse = _mapper.Map<UserResponse>(user);
            return Ok(userResponse);
        }

        [HttpGet("by-role/{roleName}")]
        public async Task<IActionResult> GetUsersByRole(string roleName)
        {
            var users = await _userService.GetUsersByRoleNameAsync(roleName);
            var userResponses = _mapper.Map<List<UserResponse>>(users);
            return Ok(userResponses);
        }

    }
}
