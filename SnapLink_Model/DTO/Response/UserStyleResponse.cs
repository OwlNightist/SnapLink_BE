using System;
using System.Collections.Generic;

namespace SnapLink_Model.DTO.Response
{
    public class UserStyleResponse
    {
        public int UserStyleId { get; set; }
        public int UserId { get; set; }
        public int StyleId { get; set; }
        public string StyleName { get; set; } = string.Empty;
        public string? StyleDescription { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserFavoriteStylesResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<UserStyleInfo> FavoriteStyles { get; set; } = new List<UserStyleInfo>();
    }

    public class UserStyleInfo
    {
        public int StyleId { get; set; }
        public string StyleName { get; set; } = string.Empty;
        public string? StyleDescription { get; set; }
        public DateTime AddedAt { get; set; }
    }

    public class StyleRecommendationResponse
    {
        public int StyleId { get; set; }
        public string StyleName { get; set; } = string.Empty;
        public string? StyleDescription { get; set; }
        public int PhotographerCount { get; set; }
        public List<RecommendedPhotographerInfo> RecommendedPhotographers { get; set; } = new List<RecommendedPhotographerInfo>();
    }

    public class RecommendedPhotographerInfo
    {
        public int PhotographerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public decimal? HourlyRate { get; set; }
        public decimal? Rating { get; set; }
        public string? AvailabilityStatus { get; set; }
        public string? ProfileImage { get; set; }
        public string? VerificationStatus { get; set; }
    }
} 