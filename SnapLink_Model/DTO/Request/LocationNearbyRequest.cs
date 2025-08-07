using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Model.DTO.Request
{
    public class LocationNearbyRequest
    {
        public string Address { get; set; } = null!;
        public double RadiusInKm { get; set; } = 5;
        public string? Tags { get; set; }        // ví dụ: "cafe,restaurant"
        public int? Limit { get; set; } = 20;
    }
}
