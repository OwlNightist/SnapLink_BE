using SnapLink_Model.DTO;

namespace SnapLink_Service.IService;

public interface IImageGenerationService
{
    Task<GeminiImageResponse> EditImageAsync(ImageEditRequest request);
    Task<GeminiImageResponse> GenerateImageAsync(GeminiImageRequest request);
}
