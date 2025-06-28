using System;
using System.Collections.Generic;

namespace SnapLink_Model.DTO.Response
{
    public class StyleResponse
    {
        public int StyleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int PhotographerCount { get; set; }
    }

    public class StyleDetailResponse : StyleResponse
    {
        public List<StylePhotographerInfo> Photographers { get; set; } = new List<StylePhotographerInfo>();
    }

    public class StylePhotographerInfo
    {
        public int PhotographerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Specialty { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal? Rating { get; set; }
        public string? AvailabilityStatus { get; set; }
        public string? ProfileImage { get; set; }
    }
} 