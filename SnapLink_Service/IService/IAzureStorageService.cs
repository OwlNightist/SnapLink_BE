using Microsoft.AspNetCore.Http;

namespace SnapLink_Service.IService
{
    public interface IAzureStorageService
    {
        Task<string> UploadImageAsync(IFormFile file, int? userId, int? photographerId, int? locationId, int? eventId);
        Task<bool> DeleteImageAsync(string blobName);
        Task<string> GetImageUrlAsync(string blobName);
        Task<bool> ImageExistsAsync(string blobName);
        string GetContainerName();
    }
} 