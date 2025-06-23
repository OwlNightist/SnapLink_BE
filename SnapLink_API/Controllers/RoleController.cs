using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Service.IService;

namespace SnapLink_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost("init-default")]
        public async Task<IActionResult> CreateDefaultRoles()
        {
            var result = await _roleService.CreateDefaultRolesAsync();
            return Ok(result);
        }
    }
}
