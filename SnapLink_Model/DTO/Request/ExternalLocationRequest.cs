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
    
} 