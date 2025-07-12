using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Response
{
    public class TransactionResponse
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public string TransactionType { get; set; } = string.Empty; // "Payment", "Withdrawal", "Transfer", "Refund"
        public decimal Amount { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // "Completed", "Pending", "Failed"
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Related data
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string? RelatedUserName { get; set; } // For transfers between users
        public string? RelatedUserEmail { get; set; }
        public int? RelatedPaymentId { get; set; } // Link to payment if this transaction is from a payment
        public int? RelatedBookingId { get; set; } // Link to booking if this transaction is from a booking
    }
} 