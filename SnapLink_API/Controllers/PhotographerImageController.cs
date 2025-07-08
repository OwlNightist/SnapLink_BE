using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO.Request;
using SnapLink_Service.IService;

namespace SnapLink_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PhotographerImageController : ControllerBase
{
    private readonly IPhotographerImageService _service;

    public PhotographerImageController(IPhotographerImageService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get photographer image by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
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
    /// Get all images for a photographer
    /// </summary>
    [HttpGet("photographer/{photographerId}")]
    public async Task<IActionResult> GetByPhotographer(int photographerId)
    {
        try
        {
            var result = await _service.GetByPhotographerAsync(photographerId);
            return Ok(result);
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
    /// Get primary image for a photographer
    /// </summary>
    [HttpGet("photographer/{photographerId}/primary")]
    public async Task<IActionResult> GetPrimaryImage(int photographerId)
    {
        try
        {
            var result = await _service.GetPrimaryImageAsync(photographerId);
            if (result == null)
                return NotFound(new { message = "No primary image found for this photographer" });
            
            return Ok(result);
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
    /// Get all photographer images with photographer details
    /// </summary>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllWithPhotographer()
    {
        try
        {
            var result = await _service.GetAllWithPhotographerAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new photographer image
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePhotographerImageRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.PhotographerImageId }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <summary>
    /// Update photographer image
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdatePhotographerImageRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.UpdateAsync(request);
            return Ok(result);
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
    /// Set image as primary
    /// </summary>
    [HttpPatch("{id}/set-primary")]
    public async Task<IActionResult> SetAsPrimary(int id)
    {
        try
        {
            var result = await _service.SetAsPrimaryAsync(id);
            if (!result)
            {
                return NotFound(new { message = $"Photographer image with ID {id} not found" });
            }
            return Ok(new { message = "Image set as primary successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete photographer image
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
            {
                return NotFound(new { message = $"Photographer image with ID {id} not found" });
            }
            return Ok(new { message = "Image deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }
} 