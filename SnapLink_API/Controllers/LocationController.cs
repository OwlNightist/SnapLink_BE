using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO;
using SnapLink_Model.DTO.Request;
using SnapLink_Service.IService;
using SnapLink_Service.Service;

namespace SnapLink_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _service;
        public LocationController(ILocationService service)
        {
            _service = service;
        }
        [HttpGet("GetAllLocations")]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("GetLocationById")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost("CreateLocation")]
        public async Task<IActionResult> Create(LocationDto dto)
        {
            await _service.CreateAsync(dto);
            return Ok("Created");
        }

        [HttpPut("UpdateLocation")]
        public async Task<IActionResult> Update(int id, LocationDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok("Updated");
        }

        [HttpDelete("DeleteLocation")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok("Deleted");
        }
      
        [HttpPost("nearby/combined")]
        public async Task<IActionResult> GetNearbyCombined([FromBody] LocationNearbyRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Address) || req.RadiusInKm <= 0)
                return BadRequest("Address và RadiusInKm là bắt buộc.");

            var data = await _service.GetNearbyCombinedAsync(req.Address, req.RadiusInKm, req.Tags, req.Limit ?? 20);
            return Ok(data);
        }
        [HttpPut("update-coordinates/{id:int}")]
        public async Task<IActionResult> UpdateCoordinates(int id)
        {
            await _service.UpdateCoordinatesByAddressAsync(id);
            return Ok("Coordinates updated.");
        }

    }
}
