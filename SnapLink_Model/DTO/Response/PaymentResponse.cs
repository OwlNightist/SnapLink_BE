namespace SnapLink_Model.DTO.Response;

public class PaymentResponse
{
    public int Error { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
}

public class PaymentDetailsResponse
{
    public int PaymentId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int BookingId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ExternalTransactionId { get; set; }
    public string? Method { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Related booking info
    public int PhotographerId { get; set; }
    public string PhotographerName { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    
    // Transaction summary
    public List<TransactionSummaryResponse> Transactions { get; set; } = new List<TransactionSummaryResponse>();
}

public class TransactionSummaryResponse
{
    public int TransactionId { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? FromUserName { get; set; }
    public string? ToUserName { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
} 