using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Request;

public class CreateTransactionRequest
{
    [Required]
    public int? ReferencePaymentId { get; set; }
    
    [Required]
    public int? FromUserId { get; set; }
    
    [Required]
    public int? ToUserId { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }
    
    public string Currency { get; set; } = "VND";
    
    [Required]
    public string Type { get; set; } = string.Empty; // TransactionType enum
    
    public string Status { get; set; } = "Success"; // TransactionStatus enum
    
    public string? Note { get; set; }
}

public class UpdateTransactionRequest
{
    public string? Status { get; set; }
    public string? Note { get; set; }
} 