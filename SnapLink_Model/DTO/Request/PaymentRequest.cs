using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Request;

public class CreatePaymentLinkRequest
{
    [Required]
    public string ProductName { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public string CancelUrl { get; set; } = string.Empty;
    
    [Required]
    public string ReturnUrl { get; set; } = string.Empty;
    
    [Required]
    public int PhotographerId { get; set; }
    
    [Required]
    public int LocationId { get; set; }
}

public class ConfirmWebhookRequest
{
    [Required]
    public string WebhookUrl { get; set; } = string.Empty;
} 