using System;

namespace SnapLink_Repository.Entity
{
    public class Image
    {
        public int Id { get; set; }
        public string Url { get; set; } // or Path, or whatever you use
        public string Type { get; set; } // e.g., "photographer", "location"
        public int RefId { get; set; } // The referenced entity's ID
        public bool IsPrimary { get; set; }
        public string? Caption { get; set; } // Optional caption for the image
        public DateTime CreatedAt { get; set; }
    }
} 