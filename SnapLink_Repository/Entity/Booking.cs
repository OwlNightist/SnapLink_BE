using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class Booking
{
    public int BookingId { get; set; }

    public int UserId { get; set; }

    public int PhotographerId { get; set; }

    public int LocationId { get; set; }

    public int? EventId { get; set; }

    public DateTime? StartDatetime { get; set; }

    public DateTime? EndDatetime { get; set; }

    public string? Status { get; set; }

    public string? SpecialRequests { get; set; }

    public decimal? TotalPrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

    public virtual PhotographerEvent? Event { get; set; }

    public virtual Location Location { get; set; } = null!;

    public virtual Payment? Payment { get; set; }

    public virtual Photographer Photographer { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual User User { get; set; } = null!;
}
