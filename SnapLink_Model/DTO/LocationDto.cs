using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Model.DTO
{
    public class LocationDto
    {
        public int? LocationOwnerId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public string? Amenities { get; set; }
        public decimal? HourlyRate { get; set; }
        public int? Capacity { get; set; }
        public bool? Indoor { get; set; }
        public bool? Outdoor { get; set; }
        public string? AvailabilityStatus { get; set; }
        public bool? FeaturedStatus { get; set; }
        public string? VerificationStatus { get; set; }
    }
}
