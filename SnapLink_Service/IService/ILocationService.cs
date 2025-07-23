using SnapLink_Repository.Entity;
using SnapLink_Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Service.IService
{
    public interface ILocationService
    {
        Task<IEnumerable<Location>> GetAllAsync();
        Task<Location?> GetByIdAsync(int id);
        Task CreateAsync(LocationDto dto);
        Task UpdateAsync(int id, LocationDto dto);
        Task DeleteAsync(int id);
        
        // New location-based search methods
        Task<IEnumerable<Location>> GetLocationsWithinRadiusAsync(double latitude, double longitude, double radiusKm);
        Task<double> CalculateDistanceAsync(double lat1, double lon1, double lat2, double lon2);
    }
}
