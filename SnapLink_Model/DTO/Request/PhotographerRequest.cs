using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Request
{
    public class CreatePhotographerRequest
    {
        [Required]
        public int UserId { get; set; }

        public int? YearsExperience { get; set; }

        [MaxLength(500)]
        public string? Equipment { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? HourlyRate { get; set; }

        [MaxLength(30)]
        public string? AvailabilityStatus { get; set; }

        [Range(0, 5)]
        public decimal? Rating { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? RatingSum { get; set; }

        [Range(0, int.MaxValue)]
        public int? RatingCount { get; set; }

        public bool? FeaturedStatus { get; set; }

        [MaxLength(30)]
        public string? VerificationStatus { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(500)]
        public string? GoogleMapsAddress { get; set; }

        [Range(-90, 90)]
        public double? Latitude { get; set; }

        [Range(-180, 180)]
        public double? Longitude { get; set; }
        
        // Style IDs for many-to-many relationship
        public List<int> StyleIds { get; set; } = new List<int>();
    }

    public class UpdatePhotographerRequest
    {
        public int? YearsExperience { get; set; }

        [MaxLength(500)]
        public string? Equipment { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? HourlyRate { get; set; }

        [MaxLength(30)]
        public string? AvailabilityStatus { get; set; }

        [Range(0, 5)]
        public decimal? Rating { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? RatingSum { get; set; }

        [Range(0, int.MaxValue)]
        public int? RatingCount { get; set; }

        public bool? FeaturedStatus { get; set; }

        [MaxLength(30)]
        public string? VerificationStatus { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(500)]
        public string? GoogleMapsAddress { get; set; }

        [Range(-90, 90)]
        public double? Latitude { get; set; }

        [Range(-180, 180)]
        public double? Longitude { get; set; }
        
        // Style IDs for many-to-many relationship
        public List<int> StyleIds { get; set; } = new List<int>();
    }
} 