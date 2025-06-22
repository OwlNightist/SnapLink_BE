using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int BookingId { get; set; }

    public decimal? Amount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? Status { get; set; }

    public string? TransactionId { get; set; }

    public decimal? PhotographerPayoutAmount { get; set; }

    public decimal? LocationOwnerPayoutAmount { get; set; }

    public decimal? PlatformFee { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();

    public virtual ICollection<PremiumSubscription> PremiumSubscriptions { get; set; } = new List<PremiumSubscription>();
}
