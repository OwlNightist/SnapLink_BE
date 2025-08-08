using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Model.DTO.Response
{
    public class NearbyCombinedItem
    {
        public string Source { get; set; } = "internal";
        public int? LocationId { get; set; }
        public string? ExternalId { get; set; }  
        public string? Class { get; set; }
        public string? Type { get; set; }

        
        public string? Name { get; set; }
        public string? Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double DistanceInKm { get; set; }
    }
}
