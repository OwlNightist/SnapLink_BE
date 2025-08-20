using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Request
{
    public class CreateWithdrawalRequest
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Bank account number cannot exceed 100 characters")]
        public string BankAccountNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "Bank account name cannot exceed 100 characters")]
        public string BankAccountName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "Bank name cannot exceed 100 characters")]
        public string BankName { get; set; } = string.Empty;
    }

    public class UpdateWithdrawalRequest
    {
        [StringLength(100, ErrorMessage = "Bank account number cannot exceed 100 characters")]
        public string? BankAccountNumber { get; set; }

        [StringLength(100, ErrorMessage = "Bank account name cannot exceed 100 characters")]
        public string? BankAccountName { get; set; }

        [StringLength(100, ErrorMessage = "Bank name cannot exceed 100 characters")]
        public string? BankName { get; set; }
    }

    public class ProcessWithdrawalRequest
    {
        [Required]
        public string Status { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Rejection reason cannot exceed 500 characters")]
        public string? RejectionReason { get; set; }
    }
}
