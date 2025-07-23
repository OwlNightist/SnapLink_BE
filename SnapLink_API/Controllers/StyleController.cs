using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO.Request;
using SnapLink_Service.IService;

namespace SnapLink_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StyleController : ControllerBase
    {
        private readonly IStyleService _styleService;

        public StyleController(IStyleService styleService)
        {
            _styleService = styleService;
        }

        /// <summary>
        /// Get all styles
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllStyles()
        {
            try
            {
                var styles = await _styleService.GetAllStylesAsync();
                return Ok(styles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get style by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStyleById(int id)
        {
            try
            {
                var style = await _styleService.GetStyleByIdAsync(id);
                return Ok(style);
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
        /// Get style with detailed information including photographers
        /// </summary>
        [HttpGet("{id}/detail")]
        public async Task<IActionResult> GetStyleDetail(int id)
        {
            try
            {
                var style = await _styleService.GetStyleDetailAsync(id);
                return Ok(style);
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
        /// Get styles by name (search)
        /// </summary>
        [HttpGet("search/{name}")]
        public async Task<IActionResult> GetStylesByName(string name)
        {
            try
            {
                var styles = await _styleService.GetStylesByNameAsync(name);
                return Ok(styles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get popular styles (with most photographers)
        /// </summary>
        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularStyles([FromQuery] int count = 10)
        {
            try
            {
                var styles = await _styleService.GetPopularStylesAsync(count);
                return Ok(styles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new style
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateStyle([FromBody] CreateStyleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var style = await _styleService.CreateStyleAsync(request);
                return CreatedAtAction(nameof(GetStyleById), new { id = style.StyleId }, style);
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
        /// Update style
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStyle(int id, [FromBody] UpdateStyleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var style = await _styleService.UpdateStyleAsync(id, request);
                return Ok(style);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
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
        /// Delete style
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStyle(int id)
        {
            try
            {
                var result = await _styleService.DeleteStyleAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"Style with ID {id} not found" });
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get styles with photographer count
        /// </summary>
        [HttpGet("with-count")]
        public async Task<IActionResult> GetStylesWithPhotographerCount()
        {
            try
            {
                var styles = await _styleService.GetStylesWithPhotographerCountAsync();
                return Ok(styles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
} 