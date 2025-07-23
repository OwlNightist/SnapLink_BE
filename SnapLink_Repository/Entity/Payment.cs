using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int CustomerId { get; set; }

    public int BookingId { get; set; }

    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? ExternalTransactionId { get; set; }

    public string? Method { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();

    public virtual Booking Booking { get; set; } = null!;

    public virtual User Customer { get; set; } = null!;

    public virtual ICollection<PremiumSubscription> PremiumSubscriptions { get; set; } = new List<PremiumSubscription>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
