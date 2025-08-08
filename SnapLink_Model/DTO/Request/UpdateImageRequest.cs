using System;

namespace SnapLink_Model.DTO.Request
{
    public class UpdateImageRequest
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? PhotographerId { get; set; }
        public int? LocationId { get; set; }
        public string? Url { get; set; }
        public bool? IsPrimary { get; set; }
        public string? Caption { get; set; }
    }
} 