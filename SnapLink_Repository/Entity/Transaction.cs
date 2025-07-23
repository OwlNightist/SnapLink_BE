using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int? ReferencePaymentId { get; set; }

    public int? FromUserId { get; set; }

    public int? ToUserId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User? FromUser { get; set; }

    public virtual Payment? ReferencePayment { get; set; }

    public virtual User? ToUser { get; set; }
}
