using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Request;

public class ExternalLocationRequest
{
    [Required]
    public string PlaceId { get; set; } = string.Empty; // Google Places ID
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Address { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public double? Latitude { get; set; }
    
    public double? Longitude { get; set; }
    
    public string? PhotoReference { get; set; } // Google Places photo reference
    
    public string? Types { get; set; } // Comma-separated place types from Google
} 