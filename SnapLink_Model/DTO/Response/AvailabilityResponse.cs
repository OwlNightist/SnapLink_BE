using System;

namespace SnapLink_Model.DTO.Response
{
    public class AvailabilityResponse
    {
        public int AvailabilityId { get; set; }
        public int PhotographerId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class AvailabilityDetailResponse : AvailabilityResponse
    {
        public string? PhotographerName { get; set; }
        public string? PhotographerEmail { get; set; }
    }

    public class PhotographerAvailabilityResponse
    {
        public int PhotographerId { get; set; }
        public string? PhotographerName { get; set; }
        public List<AvailabilityResponse> Availabilities { get; set; } = new List<AvailabilityResponse>();
    }

    public class AvailabilityCheckResponse
    {
        public int PhotographerId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public string? Message { get; set; }
        public List<AvailabilityResponse> ConflictingAvailabilities { get; set; } = new List<AvailabilityResponse>();
    }

    public class AvailableTimeSlotResponse
    {
        public int PhotographerId { get; set; }
        public DateTime Date { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public List<TimeSlot> AvailableSlots { get; set; } = new List<TimeSlot>();
        public List<TimeSlot> BookedSlots { get; set; } = new List<TimeSlot>();
        public List<TimeSlot> RegisteredAvailability { get; set; } = new List<TimeSlot>();
    }

    public class TimeSlot
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } = string.Empty; // "Available", "Booked", "Registered"
        public string? BookingInfo { get; set; } // Additional info for booked slots
    }
} 