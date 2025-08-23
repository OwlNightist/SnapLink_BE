using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO.Request;
using SnapLink_Service.IService;

namespace SnapLink_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotographerController : ControllerBase
    {
        private readonly IPhotographerService _photographerService;
        private readonly IPhotographerLocationService _locationService;

        public PhotographerController(IPhotographerService photographerService, IPhotographerLocationService locationService)
        {
            _photographerService = photographerService;
            _locationService = locationService;
        }

        [HttpPut("{id}/location")]
        public async Task<IActionResult> UpdatePhotographerLocation(int id, [FromBody] UpdatePhotographerLocationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _locationService.UpdatePhotographerLocationAsync(id, request.Address, request.GoogleMapsAddress, request.Latitude, request.Longitude);
                return Ok(new { message = "Location updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPhotographers()
        {
            try
            {
                var photographers = await _photographerService.GetAllPhotographersAsync();
                return Ok(photographers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetPhotographerById(int id)
        {
            try
            {
                var photographer = await _photographerService.GetPhotographerByIdAsync(id);
                return Ok(photographer);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("{id}/detail")]
        public async Task<IActionResult> GetPhotographerDetail(int id)
        {
            try
            {
                var photographer = await _photographerService.GetPhotographerDetailAsync(id);
                return Ok(photographer);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }


        [HttpGet("style/{styleName}")]
        public async Task<IActionResult> GetPhotographersByStyle(string styleName)
        {
            try
            {
                var photographers = await _photographerService.GetPhotographersByStyleAsync(styleName);
                return Ok(photographers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreatePhotographer([FromBody] CreatePhotographerRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var photographer = await _photographerService.CreatePhotographerAsync(request);
                return CreatedAtAction(nameof(GetPhotographerById), new { id = photographer.PhotographerId }, photographer);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePhotographer(int id, [FromBody] UpdatePhotographerRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var photographer = await _photographerService.UpdatePhotographerAsync(id, request);
                return Ok(photographer);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhotographer(int id)
        {
            try
            {
                var result = await _photographerService.DeletePhotographerAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"Photographer with ID {id} not found" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }


        [HttpPatch("{id}/availability")]
        public async Task<IActionResult> UpdateAvailability(int id, [FromBody] string availabilityStatus)
        {
            try
            {
                var result = await _photographerService.UpdateAvailabilityAsync(id, availabilityStatus);
                if (!result)
                {
                    return NotFound(new { message = $"Photographer with ID {id} not found" });
                }
                return Ok(new { message = "Availability updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }


        [HttpPatch("{id}/rating")]
        public async Task<IActionResult> UpdateRating(int id, [FromBody] decimal rating)
        {
            try
            {
                var result = await _photographerService.UpdateRatingAsync(id, rating);
                if (!result)
                {
                    return NotFound(new { message = $"Photographer with ID {id} not found" });
                }
                return Ok(new { message = "Rating updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }


        [HttpPatch("{id}/verify")]
        public async Task<IActionResult> VerifyPhotographer(int id, [FromBody] string verificationStatus)
        {
            try
            {
                var result = await _photographerService.VerifyPhotographerAsync(id, verificationStatus);
                if (!result)
                {
                    return NotFound(new { message = $"Photographer with ID {id} not found" });
                }
                return Ok(new { message = "Verification status updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }


        [HttpGet("{id}/styles")]
        public async Task<IActionResult> GetPhotographerStyles(int id)
        {
            try
            {
                var styles = await _photographerService.GetPhotographerStylesAsync(id);
                return Ok(styles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("{id}/styles/{styleId}")]
        public async Task<IActionResult> AddStyleToPhotographer(int id, int styleId)
        {
            try
            {
                var result = await _photographerService.AddStyleToPhotographerAsync(id, styleId);
                if (!result)
                {
                    return BadRequest(new { message = "Failed to add style. Photographer or style not found, or style already exists." });
                }
                return Ok(new { message = "Style added successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("{id}/styles/{styleId}")]
        public async Task<IActionResult> RemoveStyleFromPhotographer(int id, int styleId)
        {
            try
            {
                var result = await _photographerService.RemoveStyleFromPhotographerAsync(id, styleId);
                if (!result)
                {
                    return BadRequest(new { message = "Failed to remove style. Photographer or style relationship not found." });
                }
                return Ok(new { message = "Style removed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("nearby")]
        public async Task<IActionResult> GetPhotographersNearby([FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] double radiusKm = 10.0)
        {
            try
            {
                if (radiusKm <= 0 || radiusKm > 100)
                {
                    return BadRequest(new { message = "Radius must be between 0 and 100 kilometers" });
                }

                var photographers = await _locationService.GetPhotographersWithinRadiusAsync(latitude, longitude, radiusKm);
                return Ok(photographers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }



    }
}
