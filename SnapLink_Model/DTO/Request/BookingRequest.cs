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
    
    // TotalPrice will be calculated automatically by the system
    // based on photographer rate, location fees, and duration
}

public class UpdateBookingRequest
{
    public DateTime? StartDatetime { get; set; }
    
    public DateTime? EndDatetime { get; set; }
    
    public string? SpecialRequests { get; set; }
    
    public string? Status { get; set; }
    
    // TotalPrice is calculated automatically and cannot be manually updated
} 