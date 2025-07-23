using Microsoft.AspNetCore.Http;

namespace SnapLink_Model.DTO.Request
{
    public class UploadImageRequest
    {
        public IFormFile File { get; set; } = null!;
        public string Type { get; set; } = string.Empty; // e.g., "photographer", "location"
        public int RefId { get; set; } // The referenced entity's ID
        public bool IsPrimary { get; set; } = false;
        public string? Caption { get; set; }
    }
} 