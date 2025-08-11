using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO;
using SnapLink_Service.IService;

namespace SnapLink_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageGenerationController : ControllerBase
{
    private readonly IImageGenerationService _imageGenerationService;
    private readonly ILogger<ImageGenerationController> _logger;

    public ImageGenerationController(IImageGenerationService imageGenerationService, ILogger<ImageGenerationController> logger)
    {
        _imageGenerationService = imageGenerationService;
        _logger = logger;
    }

    [HttpPost("edit")]
    public async Task<ActionResult<GeminiImageResponse>> EditImage([FromBody] ImageEditRequest request)
    {
        try
        {
            var result = await _imageGenerationService.EditImageAsync(request);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing image with prompt: {Prompt}", request.Prompt);
            
            return StatusCode(500, new GeminiImageResponse
            {
                Success = false,
                Error = $"An error occurred while editing the image: {ex.Message}"
            });
        }
    }

    [HttpPost("generate")]
    public async Task<ActionResult<GeminiImageResponse>> GenerateImage([FromBody] GeminiImageRequest request)
    {
        try
        {
            var result = await _imageGenerationService.GenerateImageAsync(request);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating image with prompt: {Prompt}", request.Prompt);
            
            return StatusCode(500, new GeminiImageResponse
            {
                Success = false,
                Error = $"An error occurred while generating the image: {ex.Message}"
            });
        }
    }
}
