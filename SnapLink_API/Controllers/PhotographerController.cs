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

        /// <summary>
        /// Get all photographers
        /// </summary>
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

        /// <summary>
        /// Get photographer by ID
        /// </summary>
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

        /// <summary>
        /// Get photographer with detailed information
        /// </summary>
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

        /// <summary>
        /// Get photographers by style
        /// </summary>
        /// <param name="styleName">Style name to filter by</param>
        /// <returns>List of photographers with the specified style</returns>
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

        /// <summary>
        /// Get available photographers
        /// </summary>
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailablePhotographers()
        {
            try
            {
                var photographers = await _photographerService.GetAvailablePhotographersAsync();
                return Ok(photographers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get featured photographers
        /// </summary>
        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedPhotographers()
        {
            try
            {
                var photographers = await _photographerService.GetFeaturedPhotographersAsync();
                return Ok(photographers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new photographer
        /// </summary>
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

        /// <summary>
        /// Update photographer
        /// </summary>
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

        /// <summary>
        /// Delete photographer
        /// </summary>
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

        /// <summary>
        /// Update photographer availability
        /// </summary>
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

        /// <summary>
        /// Update photographer rating
        /// </summary>
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

        /// <summary>
        /// Verify photographer
        /// </summary>
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

        /// <summary>
        /// Get photographer styles
        /// </summary>
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

        /// <summary>
        /// Add style to photographer
        /// </summary>
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

        /// <summary>
        /// Remove style from photographer
        /// </summary>
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

        /// <summary>
        /// Get photographers within a specified radius
        /// </summary>
        /// <param name="latitude">User's latitude</param>
        /// <param name="longitude">User's longitude</param>
        /// <param name="radiusKm">Search radius in kilometers</param>
        /// <returns>List of photographers within the specified radius</returns>
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

        /// <summary>
        /// Get photographers by city
        /// </summary>
        /// <param name="city">City name to search for</param>
        /// <returns>List of photographers in the specified city</returns>
        [HttpGet("city/{city}")]
        public async Task<IActionResult> GetPhotographersByCity(string city)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(city))
                {
                    return BadRequest(new { message = "City name cannot be empty" });
                }

                var photographers = await _locationService.GetPhotographersByCityAsync(city);
                return Ok(photographers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Calculate distance to a specific photographer
        /// </summary>
        /// <param name="photographerId">Photographer ID</param>
        /// <param name="latitude">User's latitude</param>
        /// <param name="longitude">User's longitude</param>
        /// <returns>Distance in kilometers</returns>
        [HttpGet("{photographerId}/distance")]
        public async Task<IActionResult> GetDistanceToPhotographer(int photographerId, [FromQuery] double latitude, [FromQuery] double longitude)
        {
            try
            {
                var distance = await _locationService.CalculateDistanceToPhotographerAsync(latitude, longitude, photographerId);
                return Ok(new { distance = Math.Round(distance, 2), unit = "km" });
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

        /// <summary>
        /// Update photographer location
        /// </summary>
        /// <param name="id">Photographer ID</param>
        /// <param name="request">Location update request</param>
        /// <returns>Success message</returns>
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
    }
}
