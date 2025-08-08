using SnapLink_Model.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Service.IService
{
    public interface IGeoProvider
    {
        Task<(double lat, double lon)?> GeocodeAsync(string address);
        
    }
}
