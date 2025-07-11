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

        // GET: api/image/type/{type}/ref/{refId}
        [HttpGet("type/{type}/ref/{refId}")]
        public async Task<ActionResult<IEnumerable<ImageResponse>>> GetByTypeAndRefId(string type, int refId)
        {
            var images = await _imageService.GetByTypeAndRefIdAsync(type, refId);
            return Ok(images);
        }

        // GET: api/image/type/{type}/ref/{refId}/primary
        [HttpGet("type/{type}/ref/{refId}/primary")]
        public async Task<ActionResult<ImageResponse>> GetPrimaryImage(string type, int refId)
        {
            var image = await _imageService.GetPrimaryImageAsync(type, refId);
            if (image == null)
                return NotFound($"No primary image found for {type} with ID {refId}");
            
            return Ok(image);
        }

        // GET: api/image/type/{type}
        [HttpGet("type/{type}")]
        public async Task<ActionResult<IEnumerable<ImageResponse>>> GetAllByType(string type)
        {
            var images = await _imageService.GetAllByTypeAsync(type);
            return Ok(images);
        }

        // POST: api/image
        [HttpPost]
        public async Task<ActionResult<ImageResponse>> Create([FromBody] CreateImageRequest request)
        {
            try
            {
                var image = await _imageService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = image.Id }, image);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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

        // DELETE: api/image/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _imageService.DeleteAsync(id);
            if (!result)
                return NotFound($"Image with ID {id} not found");
            
            return NoContent();
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