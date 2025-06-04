using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int UserId { get; set; }

    public decimal? Amount { get; set; }

    public string? Type { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
