using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SnapLink_Service.IService;
using System.Text.RegularExpressions;

namespace SnapLink_Service.Service
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        private readonly BlobContainerClient _containerClient;

        public AzureStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureStorage:ConnectionString"];
            _containerName = configuration["AzureStorage:ContainerName"] ?? "images";
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Azure Storage connection string is not configured");
            }

            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            
            // Ensure container exists
            _containerClient.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task<string> UploadImageAsync(IFormFile file, int? userId, int? photographerId, int? locationId)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null");

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException($"File type {fileExtension} is not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");

            // Validate file size (max 10MB)
            if (file.Length > 10 * 1024 * 1024)
                throw new ArgumentException("File size exceeds 10MB limit");

            // Determine entity type and id
            string entityType;
            int entityId;
            if (userId.HasValue)
            {
                entityType = "user";
                entityId = userId.Value;
            }
            else if (photographerId.HasValue)
            {
                entityType = "photographer";
                entityId = photographerId.Value;
            }
            else if (locationId.HasValue)
            {
                entityType = "location";
                entityId = locationId.Value;
            }
            else
            {
                throw new ArgumentException("At least one entity ID must be provided.");
            }

            // Generate unique blob name
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var sanitizedFileName = SanitizeFileName(file.FileName);
            var blobName = $"{entityType}/{entityId}/{timestamp}_{sanitizedFileName}";

            // Get blob client
            var blobClient = _containerClient.GetBlobClient(blobName);

            // Upload file
            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            // Set content type
            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = GetContentType(fileExtension)
            };
            await blobClient.SetHttpHeadersAsync(blobHttpHeaders);

            return blobName;
        }

        public async Task<bool> DeleteImageAsync(string blobName)
        {
            if (string.IsNullOrEmpty(blobName))
                return false;

            var blobClient = _containerClient.GetBlobClient(blobName);
            var response = await blobClient.DeleteIfExistsAsync();
            return response.Value;
        }

        public async Task<string> GetImageUrlAsync(string blobName)
        {
            if (string.IsNullOrEmpty(blobName))
                return string.Empty;

            var blobClient = _containerClient.GetBlobClient(blobName);
            return blobClient.Uri.ToString();
        }

        public async Task<bool> ImageExistsAsync(string blobName)
        {
            if (string.IsNullOrEmpty(blobName))
                return false;

            var blobClient = _containerClient.GetBlobClient(blobName);
            return await blobClient.ExistsAsync();
        }

        public string GetContainerName()
        {
            return _containerName;
        }

        private string SanitizeFileName(string fileName)
        {
            // Remove or replace invalid characters
            var sanitized = Regex.Replace(fileName, @"[^a-zA-Z0-9._-]", "_");
            return sanitized.Length > 100 ? sanitized.Substring(0, 100) : sanitized;
        }

        private string GetContentType(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
} 