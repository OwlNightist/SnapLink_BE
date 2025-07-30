using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SnapLink_Model.DTO.Request
{
    public class CreateAvailabilityRequest
    {
        [Required]
        public int PhotographerId { get; set; }

        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Available";
    }

    public class UpdateAvailabilityRequest
    {
        public DayOfWeek? DayOfWeek { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }
    }

    public class BulkAvailabilityRequest
    {
        [Required]
        public int PhotographerId { get; set; }

        [Required]
        public List<CreateAvailabilityRequest> Availabilities { get; set; } = new List<CreateAvailabilityRequest>();
    }

    public class CheckAvailabilityRequest
    {
        [Required]
        public int PhotographerId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
    }
} 