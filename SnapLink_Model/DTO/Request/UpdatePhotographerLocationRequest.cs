using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Request
{
    public class UpdatePhotographerLocationRequest
    {
        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(500)]
        public string? GoogleMapsAddress { get; set; }

        [Range(-90, 90)]
        public double? Latitude { get; set; }

        [Range(-180, 180)]
        public double? Longitude { get; set; }
    }
} 