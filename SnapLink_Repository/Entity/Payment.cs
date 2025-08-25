using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int CustomerId { get; set; }
    public virtual User Customer { get; set; } = null!;

    public int? BookingId { get; set; }
    public virtual Booking? Booking { get; set; }

    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = "VND";

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public string? ExternalTransactionId { get; set; }  // PayOS payment code

    public string? Method { get; set; } = "PayOS";      // Payment method

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();



    public virtual ICollection<PremiumSubscription> PremiumSubscriptions { get; set; } = new List<PremiumSubscription>();
}

public enum PaymentStatus
{
    Pending,
    Success,
    Failed,
    Cancelled
}
