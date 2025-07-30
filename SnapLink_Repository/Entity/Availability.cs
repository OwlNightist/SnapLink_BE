using System;
using System.ComponentModel.DataAnnotations;

namespace SnapLink_Repository.Entity;

public partial class Availability
{
    public int AvailabilityId { get; set; }

    public int PhotographerId { get; set; }

    [Required]
    public DayOfWeek DayOfWeek { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Available";

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public virtual Photographer Photographer { get; set; } = null!;
} 