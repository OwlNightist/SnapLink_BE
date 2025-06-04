using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class Advertisement
{
    public int AdvertisementId { get; set; }

    public int LocationId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Status { get; set; }

    public decimal? Cost { get; set; }

    public int? PaymentId { get; set; }

    public virtual Location Location { get; set; } = null!;

    public virtual Payment? Payment { get; set; }
}
