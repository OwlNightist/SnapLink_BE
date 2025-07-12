using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Response
{
    public class TransactionResponse
    {
        public int TransactionId { get; set; }
        public int? ReferencePaymentId { get; set; }
        public int? FromUserId { get; set; }
        public string? FromUserName { get; set; }
        public int? ToUserId { get; set; }
        public string? ToUserName { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // TransactionType enum
        public string Status { get; set; } = string.Empty; // TransactionStatus enum
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Related payment info
        public string? PaymentMethod { get; set; }
        public decimal? PaymentAmount { get; set; }
        public string? PaymentStatus { get; set; }
    }
} 