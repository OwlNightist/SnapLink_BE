using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SnapLink_Model.DTO.Response
{
    public class PhotographerResponse
    {
        public int PhotographerId { get; set; }
        public int UserId { get; set; }
        public int? YearsExperience { get; set; }
        public string? Equipment { get; set; }
        public string? Specialty { get; set; }
        public decimal? HourlyRate { get; set; }
        public string? AvailabilityStatus { get; set; }
        public decimal? Rating { get; set; }
        public decimal? RatingSum { get; set; }
        public int? RatingCount { get; set; }
        public bool? FeaturedStatus { get; set; }
        public string? VerificationStatus { get; set; }

        // User information
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        public string? ProfileImage { get; set; }
        public string? Bio { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? Status { get; set; }

        // Styles information
        public List<string> Styles { get; set; } = new List<string>();
    }

    public class PhotographerDetailResponse : PhotographerResponse
    {
        public int TotalBookings { get; set; }
        public int TotalReviews { get; set; }
        public decimal? AverageRating { get; set; }
        public decimal? WalletBalance { get; set; }
    }

    public class PhotographerListResponse
    {
        public int PhotographerId { get; set; }
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Specialty { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal? Rating { get; set; }
        public decimal? RatingSum { get; set; }
        public int? RatingCount { get; set; }
        public string? AvailabilityStatus { get; set; }
        public bool? FeaturedStatus { get; set; }
        public string? ProfileImage { get; set; }
        public string? VerificationStatus { get; set; }

        // Styles information
        public List<string> Styles { get; set; } = new List<string>();
    }
}