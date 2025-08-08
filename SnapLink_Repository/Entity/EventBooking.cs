using System;
using System.ComponentModel.DataAnnotations;

namespace SnapLink_Repository.Entity;

public partial class EventBooking
{
    public int EventBookingId { get; set; }

    public int EventId { get; set; }

    public int BookingId { get; set; } // Reference to existing Booking

    public int EventPhotographerId { get; set; }

    [Range(0, double.MaxValue)]
    public decimal EventPrice { get; set; } // Price for this specific event booking

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual LocationEvent Event { get; set; } = null!;

    public virtual Booking Booking { get; set; } = null!;

    public virtual EventPhotographer EventPhotographer { get; set; } = null!;
}
