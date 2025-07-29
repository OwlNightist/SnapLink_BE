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
    Purchase,           // Giao dịch chính: Người dùng thanh toán cho đơn hàng (bao gồm tiền thợ, địa điểm, và phí nền tảng)
    PhotographerFee,    // Phí phân bổ cho thợ chụp hình
    VenueFee,          // Phí phân bổ cho nơi cho thuê địa điểm
    PlatformFee,       // Phí phân bổ cho nền tảng
    Refund,            // Hoàn tiền (nếu người dùng hủy đơn hàng hoặc giao dịch thất bại)
    Deposit,           // Nạp tiền vào ví (nếu người dùng nạp tiền vào ví của mình)
    Withdrawal         // Rút tiền từ ví (nếu người dùng hoặc thợ chụp rút tiền ra ngoài)
}

public enum TransactionStatus
{
    Pending,
    Success,
    Failed
}
