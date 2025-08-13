using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mscc.GenerativeAI;
using SnapLink_Model.DTO;
using SnapLink_Service.IService;

namespace SnapLink_Service.Service;

public class ImageGenerationService : IImageGenerationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ImageGenerationService> _logger;

    public ImageGenerationService(IConfiguration configuration, ILogger<ImageGenerationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<GeminiImageResponse> EditImageAsync(ImageEditRequest request)
    {
        try
        {
            var apiKey = _configuration["GOOGLE_API_KEY"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return new GeminiImageResponse
                {
                    Success = false,
                    Error = "Google API key not configured. Please set GOOGLE_API_KEY in appsettings.json or environment variables."
                };
            }

            if (string.IsNullOrEmpty(request.ImageBase64))
            {
                return new GeminiImageResponse
                {
                    Success = false,
                    Error = "Image data is required. Please provide a base64 encoded image."
                };
            }

            var googleAI = new GoogleAI(apiKey: apiKey);
            var modelName = _configuration["GeminiAI:Model"] ?? "gemini-2.0-flash-preview-image-generation";
            var model = googleAI.GenerativeModel(model: modelName);

            // Prepare content with both text and image
            var generateRequest = new GenerateContentRequest(request.Prompt);
            await generateRequest.AddMedia(request.ImageBase64, request.MimeType);
            Console.WriteLine("vai cut mode ne "+ modelName);
            // Set generation config with both TEXT and IMAGE modalities
            generateRequest.GenerationConfig = new GenerationConfig
            {
                ResponseModalities = new List<ResponseModality> { ResponseModality.Text, ResponseModality.Image }
            };
            
            // Generate content with image editing
            var response = await model.GenerateContent(generateRequest);

            var result = new GeminiImageResponse
            {
                Success = true,
                Message = "Image edited successfully"
            };

            foreach (var part in response.Candidates[0].Content.Parts)
            {
                if (!string.IsNullOrEmpty(part.Text))
                {
                    result.TextResponse = part.Text;
                }
                else if (part.InlineData != null)
                {
                    result.ImageBase64 = part.InlineData.Data;
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing image with prompt: {Prompt}", request.Prompt);
            
            return new GeminiImageResponse
            {
                Success = false,
                Error = $"An error occurred while editing the image: {ex.Message}"
            };
        }
    }

    public async Task<GeminiImageResponse> GenerateImageAsync(GeminiImageRequest request)
    {
        try
        {
            // Get API key from configuration
            var apiKey = _configuration["GOOGLE_API_KEY"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return new GeminiImageResponse
                {
                    Success = false,
                    Error = "Google API key not configured. Please set GOOGLE_API_KEY in appsettings.json or environment variables."
                };
            }

            // Initialize Google AI with API key
            var googleAI = new GoogleAI(apiKey: apiKey);
            var modelName = _configuration["GeminiAI:Model"] ?? "gemini-2.0-flash-preview-image-generation";
            var model = googleAI.GenerativeModel(model: modelName);

            // Create generation config with both TEXT and IMAGE modalities
            var generationConfig = new GenerationConfig
            {
                ResponseModalities = new List<ResponseModality> { ResponseModality.Text, ResponseModality.Image }
            };

            // Generate content with explicit response modalities
            var response = await model.GenerateContent(request.Prompt, generationConfig);

            var result = new GeminiImageResponse
            {
                Success = true,
                Message = "Image generated successfully"
            };

            // Process the response parts
            foreach (var part in response.Candidates[0].Content.Parts)
            {
                if (!string.IsNullOrEmpty(part.Text))
                {
                    result.TextResponse = part.Text;
                }
                else if (part.InlineData != null)
                {
                    // Convert image data to base64 for API response
                    result.ImageBase64 = part.InlineData.Data;
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating image with prompt: {Prompt}", request.Prompt);
            
            return new GeminiImageResponse
            {
                Success = false,
                Error = $"An error occurred while generating the image: {ex.Message}"
            };
        }
    }
}
