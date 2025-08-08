using System;
using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Request
{
    public class CreateLocationEventRequest
    {
        [Required]
        public int LocationId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? DiscountedPrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? OriginalPrice { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxPhotographers { get; set; } = 10;

        [Range(1, int.MaxValue)]
        public int MaxBookingsPerSlot { get; set; } = 5;
    }

    public class UpdateLocationEventRequest
    {
        [MaxLength(255)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? DiscountedPrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? OriginalPrice { get; set; }

        [Range(1, int.MaxValue)]
        public int? MaxPhotographers { get; set; }

        [Range(1, int.MaxValue)]
        public int? MaxBookingsPerSlot { get; set; }

        [MaxLength(30)]
        public string? Status { get; set; }
    }

    public class EventApplicationRequest
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public int PhotographerId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? SpecialRate { get; set; }
    }

    public class EventApplicationResponseRequest
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public int PhotographerId { get; set; }

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = string.Empty; // "Approved" or "Rejected"

        [MaxLength(500)]
        public string? RejectionReason { get; set; }
    }

    public class EventBookingRequest
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public int EventPhotographerId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime StartDatetime { get; set; }

        [Required]
        public DateTime EndDatetime { get; set; }

        [MaxLength(1000)]
        public string? SpecialRequests { get; set; }
    }
}
