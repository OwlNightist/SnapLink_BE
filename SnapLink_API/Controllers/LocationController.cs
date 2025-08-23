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

        [HttpGet("registered-nearby")]
        public async Task<IActionResult> GetRegisteredLocationsNearby([FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] double radiusKm = 50.0)
        {
            try
            {
                // Validate latitude and longitude
                if (latitude < -90 || latitude > 90)
                {
                    return BadRequest(new { message = "Latitude must be between -90 and 90" });
                }

                if (longitude < -180 || longitude > 180)
                {
                    return BadRequest(new { message = "Longitude must be between -180 and 180" });
                }

                // Validate radius
                if (radiusKm <= 0 || radiusKm > 200)
                {
                    return BadRequest(new { message = "Radius must be between 0 and 200 kilometers" });
                }

                var locations = await _service.GetRegisteredLocationsNearbyAsync(latitude, longitude, radiusKm);
                
                return Ok(new
                {
                    message = "Registered locations retrieved successfully",
                    data = locations,
                    count = locations.Count,
                    searchParameters = new
                    {
                        latitude = latitude,
                        longitude = longitude,
                        radiusKm = radiusKm
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

    }
}
