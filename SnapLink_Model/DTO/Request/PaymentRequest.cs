using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Request;

public class CreatePaymentLinkRequest
{
    [Required]
    public string ProductName { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public int BookingId { get; set; }
}