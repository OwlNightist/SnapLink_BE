using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class WithdrawalRequest
{
    public int Id { get; set; }

    public int WalletId { get; set; }

    public decimal Amount { get; set; }

    public string? BankAccountNumber { get; set; }

    public string? BankAccountName { get; set; }

    public string? BankName { get; set; }

    public string? RequestStatus { get; set; }

    public DateTime? RequestedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public int? ProcessedByModeratorId { get; set; }

    public string? RejectionReason { get; set; }

    public virtual Wallet Wallet { get; set; } = null!;
}
