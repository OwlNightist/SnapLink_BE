using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO;
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
        [HttpPost("nearby")]
        public async Task<IActionResult> GetNearbyLocations([FromBody] AddressRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Address))
                return BadRequest("Address is required.");

            var locations = await _service.GetNearbyLocationsAsync(request.Address);
            return Ok(locations);
        }
        [HttpPut("update-location-coordinates")]
        public async Task<IActionResult> UpdateCoordinates([FromBody] UpdateLocationCoordinatesRequest request)
        {
            try
            {
                await _service.UpdateCoordinatesAsync(request.LocationId);
                return Ok("Coordinates updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
