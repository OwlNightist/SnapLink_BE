using System;

namespace SnapLink_Model.DTO.Response
{
    public class ImageResponse
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int? PhotographerId { get; set; }
        public int? LocationId { get; set; }
        public int? PhotographerEventId { get; set; }
        public bool IsPrimary { get; set; }
        public string? Caption { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 