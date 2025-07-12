using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Request;

public class CreateBookingRequest
{
    [Required]
    public int PhotographerId { get; set; }
    
    // LocationId can be null for external locations
    public int? LocationId { get; set; }
    
    // External location details (used when LocationId is null)
    public ExternalLocationRequest? ExternalLocation { get; set; }
    
    [Required]
    public DateTime StartDatetime { get; set; }
    
    [Required]
    public DateTime EndDatetime { get; set; }
    
    public string? SpecialRequests { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Total price must be greater than 0")]
    public decimal TotalPrice { get; set; }
}

public class UpdateBookingRequest
{
    public DateTime? StartDatetime { get; set; }
    
    public DateTime? EndDatetime { get; set; }
    
    public string? SpecialRequests { get; set; }
    
    public string? Status { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Total price must be greater than 0")]
    public decimal? TotalPrice { get; set; }
} 