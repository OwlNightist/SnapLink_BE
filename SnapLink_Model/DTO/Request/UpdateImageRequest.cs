using System;

namespace SnapLink_Model.DTO.Request
{
    public class UpdateImageRequest
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty; // e.g., "photographer", "location"
        public string? Url { get; set; }
        public bool? IsPrimary { get; set; }
        public string? Caption { get; set; }
    }
} 