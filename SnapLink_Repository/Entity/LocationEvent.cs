using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SnapLink_Repository.Entity;

public partial class LocationEvent
{
    public int EventId { get; set; }

    public int LocationId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? DiscountedPrice { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? OriginalPrice { get; set; }

    [Range(1, int.MaxValue)]
    public int MaxPhotographers { get; set; } = 10;

    [Range(1, int.MaxValue)]
    public int MaxBookingsPerSlot { get; set; } = 5;

    [MaxLength(30)]
    public string Status { get; set; } = "Draft"; // "Draft", "Open", "Active", "Closed", "Cancelled"

    [MaxLength(500)]
    public string? EventImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Location Location { get; set; } = null!;

    public virtual ICollection<EventPhotographer> EventPhotographers { get; set; } = new List<EventPhotographer>();

    public virtual ICollection<EventBooking> EventBookings { get; set; } = new List<EventBooking>();
}
