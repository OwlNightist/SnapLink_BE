using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class PhotographerEvent
{
    public int EventId { get; set; }

    public int PhotographerId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public decimal? OriginalPrice { get; set; }

    public decimal? DiscountedPrice { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? MaxBookings { get; set; }

    public int? CurrentBookings { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual Photographer Photographer { get; set; } = null!;

    public virtual ICollection<PhotographerEventLocation> PhotographerEventLocations { get; set; } = new List<PhotographerEventLocation>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();
} 