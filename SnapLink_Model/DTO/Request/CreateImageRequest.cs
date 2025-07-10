using System;

namespace SnapLink_Model.DTO.Request
{
    public class CreateImageRequest
    {
        public string Url { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // e.g., "photographer", "location"
        public int RefId { get; set; } // The referenced entity's ID
        public bool IsPrimary { get; set; } = false;
        public string? Caption { get; set; }
    }
} 