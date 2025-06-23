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
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int PhotographerId { get; set; }
    public string PhotographerName { get; set; } = string.Empty;
    public decimal PhotographerPayoutAmount { get; set; }
    public decimal PlatformFee { get; set; }
} 