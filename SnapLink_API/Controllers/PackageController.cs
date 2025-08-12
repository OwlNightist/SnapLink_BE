using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO;
using SnapLink_Service.IService;

namespace SnapLink_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _svc;
        public PackageController(IPackageService svc) => _svc = svc;

        [HttpPost("CreatePackage")]
        public async Task<IActionResult> Create([FromBody] CreatePackageDto dto)
        {
            try { var id = await _svc.CreateAsync(dto); return Ok(new { packageId = id, message = "Created" }); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("UpdatePackage/{packageId:int}")]
        public async Task<IActionResult> Update(int packageId, [FromBody] UpdatePackageDto dto)
        {
            try { await _svc.UpdateAsync(packageId, dto); return Ok("Updated"); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("DeletePackage/{packageId:int}")]
        public async Task<IActionResult> Delete(int packageId)
        {
            try { await _svc.DeleteAsync(packageId); return Ok("Deleted"); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("GetPackage/{packageId:int}")]
        public async Task<IActionResult> GetById(int packageId)
        {
            var p = await _svc.GetByIdAsync(packageId);
            return p == null ? NotFound() : Ok(p);
        }

        [HttpGet("GetPackages")]
        public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());
    }
}
