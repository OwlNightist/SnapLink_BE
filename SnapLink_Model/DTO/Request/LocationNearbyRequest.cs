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
        public double RadiusInKm { get; set; }            
    }
}
