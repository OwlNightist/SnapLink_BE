using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Service.IService;

namespace SnapLink_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        // GET: api/image/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ImageResponse>> GetById(int id)
        {
            try
            {
                var image = await _imageService.GetByIdAsync(id);
                return Ok(image);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/image/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ImageResponse>>> GetByUserId(int userId)
        {
            var images = await _imageService.GetByUserIdAsync(userId);
            return Ok(images);
        }

        // GET: api/image/photographer/{photographerId}
        [HttpGet("photographer/{photographerId}")]
        public async Task<ActionResult<IEnumerable<ImageResponse>>> GetByPhotographerId(int photographerId)
        {
            var images = await _imageService.GetByPhotographerIdAsync(photographerId);
            return Ok(images);
        }

        // GET: api/image/location/{locationId}
        [HttpGet("location/{locationId}")]
        public async Task<ActionResult<IEnumerable<ImageResponse>>> GetByLocationId(int locationId)
        {
            var images = await _imageService.GetByLocationIdAsync(locationId);
            return Ok(images);
        }

        // GET: api/image/event/{eventId}
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<ImageResponse>>> GetByEventId(int eventId)
        {
            var images = await _imageService.GetByEventIdAsync(eventId);
            return Ok(images);
        }

        // GET: api/image/user/{userId}/primary
        [HttpGet("user/{userId}/primary")]
        public async Task<ActionResult<ImageResponse>> GetPrimaryByUserId(int userId)
        {
            var image = await _imageService.GetPrimaryByUserIdAsync(userId);
            if (image == null)
                return NotFound($"No primary image found for user with ID {userId}");
            return Ok(image);
        }

        // GET: api/image/photographer/{photographerId}/primary
        [HttpGet("photographer/{photographerId}/primary")]
        public async Task<ActionResult<ImageResponse>> GetPrimaryByPhotographerId(int photographerId)
        {
            var image = await _imageService.GetPrimaryByPhotographerIdAsync(photographerId);
            if (image == null)
                return NotFound($"No primary image found for photographer with ID {photographerId}");
            return Ok(image);
        }

        // GET: api/image/location/{locationId}/primary
        [HttpGet("location/{locationId}/primary")]
        public async Task<ActionResult<ImageResponse>> GetPrimaryByLocationId(int locationId)
        {
            var image = await _imageService.GetPrimaryByLocationIdAsync(locationId);
            if (image == null)
                return NotFound($"No primary image found for location with ID {locationId}");
            return Ok(image);
        }

        // GET: api/image/event/{eventId}/primary
        [HttpGet("event/{eventId}/primary")]
        public async Task<ActionResult<ImageResponse>> GetPrimaryByEventId(int eventId)
        {
            var image = await _imageService.GetPrimaryByEventIdAsync(eventId);
            if (image == null)
                return NotFound($"No primary image found for event with ID {eventId}");
            return Ok(image);
        }

        // POST: api/image
        [HttpPost]
        public async Task<ActionResult<ImageResponse>> UploadImage([FromForm] UploadImageRequest request)
        {
            try
            {
                if (request.File == null || request.File.Length == 0)
                    return BadRequest("No file provided");

                var image = await _imageService.UploadImageAsync(request);
                // return CreatedAtAction(nameof(GetById), new { id = image.Id }, image);
                return Ok(image);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/image
        [HttpPut]
        public async Task<ActionResult<ImageResponse>> Update([FromBody] UpdateImageRequest request)
        {
            try
            {
                var image = await _imageService.UpdateAsync(request);
                return Ok(image);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE: api/image/{id} (Soft Delete)
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _imageService.DeleteAsync(id);
            if (!result)
                return NotFound($"Image with ID {id} not found");
            
            return NoContent();
        }

        // DELETE: api/image/{id}/hard (Hard Delete - Permanent)
        [HttpDelete("{id}/hard")]
        public async Task<ActionResult> HardDelete(int id)
        {
            var result = await _imageService.HardDeleteAsync(id);
            if (!result)
                return NotFound($"Image with ID {id} not found");
            
            return NoContent();
        }

        // PUT: api/image/{id}/restore
        [HttpPut("{id}/restore")]
        public async Task<ActionResult> Restore(int id)
        {
            var result = await _imageService.RestoreAsync(id);
            if (!result)
                return NotFound($"Image with ID {id} not found");
            
            return NoContent();
        }

        // GET: api/image/deleted (Admin only)
        [HttpGet("deleted")]
        public async Task<ActionResult<IEnumerable<ImageResponse>>> GetDeletedImages()
        {
            var deletedImages = await _imageService.GetDeletedImagesAsync();
            return Ok(deletedImages);
        }

        // PUT: api/image/{id}/set-primary
        [HttpPut("{id}/set-primary")]
        public async Task<ActionResult> SetAsPrimary(int id)
        {
            var result = await _imageService.SetAsPrimaryAsync(id);
            if (!result)
                return NotFound($"Image with ID {id} not found");
            
            return NoContent();
        }
    }
} 