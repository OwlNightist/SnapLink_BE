using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int? ReferencePaymentId { get; set; }
    public virtual Payment? ReferencePayment { get; set; }

    public int? FromUserId { get; set; }
    public virtual User? FromUser { get; set; }

    public int? ToUserId { get; set; }
    public virtual User? ToUser { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "VND";

    public TransactionType Type { get; set; }

    public TransactionStatus Status { get; set; } = TransactionStatus.Success;

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum TransactionType
{
    Payout,       // Trả tiền cho người nhận
    Commission,   // Hoa hồng nền tảng
    Refund,       // Hoàn tiền
    Adjustment,   // Điều chỉnh số dư
    Bonus,        // Thưởng
    Topup,        // Nạp tiền
    Withdraw      // Rút tiền
}

public enum TransactionStatus
{
    Pending,
    Success,
    Failed
}
