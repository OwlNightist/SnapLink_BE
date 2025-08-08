using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Model.External
{
    public class GoogleGeocodeResponse
    {
        public GoogleGeocodeResult[] Results { get; set; }
        public string Status { get; set; }
    }

    public class GoogleGeocodeResult
    {
        public GoogleGeometry Geometry { get; set; }
    }

    public class GoogleGeometry
    {
        public GoogleLocation Location { get; set; }
    }

    public class GoogleLocation
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
