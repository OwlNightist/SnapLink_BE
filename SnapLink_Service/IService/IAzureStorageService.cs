using Microsoft.AspNetCore.Http;

namespace SnapLink_Service.IService
{
    public interface IAzureStorageService
    {
        Task<string> UploadImageAsync(IFormFile file, int? photographerId, int? locationId, int? photographerEventId);
        Task<bool> DeleteImageAsync(string blobName);
        Task<string> GetImageUrlAsync(string blobName);
        Task<bool> ImageExistsAsync(string blobName);
        string GetContainerName();
    }
} 