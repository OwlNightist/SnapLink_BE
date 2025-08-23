using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Request
{
    public class RecommendPhotographerRequest
    {
        [Required]
        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double Longitude { get; set; }

        public int? LocationId { get; set; }

        [Range(1, 100)]
        public double RadiusKm { get; set; } = 10.0;

        [Range(1, 50)]
        public int MaxResults { get; set; } = 20;
    }
}
