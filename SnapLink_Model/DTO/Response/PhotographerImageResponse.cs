using System;

namespace SnapLink_Model.DTO.Response
{
    public class PhotographerImageResponse
    {
        public int PhotographerImageId { get; set; }
        public string? ImageUrl { get; set; }
        public string? Caption { get; set; }
        public bool? IsPrimary { get; set; }
        public DateTime? UploadedAt { get; set; }
    }
} 