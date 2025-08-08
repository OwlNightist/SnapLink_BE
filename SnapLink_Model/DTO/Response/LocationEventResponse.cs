using System;
using System.Collections.Generic;

namespace SnapLink_Model.DTO.Response
{
    public class LocationEventResponse
    {
        public int EventId { get; set; }
        public int LocationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int MaxPhotographers { get; set; }
        public int MaxBookingsPerSlot { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? EventImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public LocationDto? Location { get; set; }
        public int ApprovedPhotographersCount { get; set; }
        public int TotalBookingsCount { get; set; }
        public bool IsActive => Status == "Active" && StartDate <= DateTime.UtcNow && EndDate >= DateTime.UtcNow;
        public bool IsUpcoming => Status == "Open" && StartDate > DateTime.UtcNow;
    }

    public class LocationEventDetailResponse : LocationEventResponse
    {
        public List<EventPhotographerResponse> EventPhotographers { get; set; } = new List<EventPhotographerResponse>();
        public List<EventBookingResponse> EventBookings { get; set; } = new List<EventBookingResponse>();
    }

    public class EventPhotographerResponse
    {
        public int EventPhotographerId { get; set; }
        public int EventId { get; set; }
        public int PhotographerId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime AppliedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectionReason { get; set; }
        public decimal? SpecialRate { get; set; }
        
        // Navigation properties
        public PhotographerResponse? Photographer { get; set; }
        public LocationEventResponse? Event { get; set; }
    }

    public class EventBookingResponse
    {
        public int EventBookingId { get; set; }
        public int EventId { get; set; }
        public int BookingId { get; set; }
        public int EventPhotographerId { get; set; }
        public decimal EventPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation properties
        public BookingResponse? Booking { get; set; }
        public EventPhotographerResponse? EventPhotographer { get; set; }
        public LocationEventResponse? Event { get; set; }
    }

    public class EventApplicationResponse
    {
        public int EventPhotographerId { get; set; }
        public int EventId { get; set; }
        public int PhotographerId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime AppliedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectionReason { get; set; }
        public decimal? SpecialRate { get; set; }
        
        // Event details
        public string EventName { get; set; } = string.Empty;
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public string EventStatus { get; set; } = string.Empty;
        
        // Photographer details
        public string PhotographerName { get; set; } = string.Empty;
        public string? PhotographerProfileImage { get; set; }
        public decimal? PhotographerRating { get; set; }
    }

    public class EventListResponse
    {
        public int EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public decimal? OriginalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? EventImageUrl { get; set; }
        public int ApprovedPhotographersCount { get; set; }
        public int TotalBookingsCount { get; set; }
        public bool IsActive { get; set; }
        public bool IsUpcoming { get; set; }
        
        // Location details
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string? LocationAddress { get; set; }
    }

    public class EventStatisticsResponse
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public int TotalApplications { get; set; }
        public int ApprovedApplications { get; set; }
        public int RejectedApplications { get; set; }
        public int PendingApplications { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageBookingValue { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public string EventStatus { get; set; } = string.Empty;
    }
}
