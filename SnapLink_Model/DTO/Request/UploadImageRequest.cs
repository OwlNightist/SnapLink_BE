using Microsoft.AspNetCore.Http;

namespace SnapLink_Model.DTO.Request
{
    public class UploadImageRequest
    {
        public IFormFile File { get; set; } = null!;
        public int? UserId { get; set; }
        public int? PhotographerId { get; set; }
        public int? LocationId { get; set; }
        public int? PhotographerEventId { get; set; }
        public bool IsPrimary { get; set; } = false;
        public string? Caption { get; set; }
    }
} 