using SnapLink_Repository.Entity;
using SnapLink_Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService
{
    public interface ILocationService
    {
        Task<IEnumerable<Location>> GetAllAsync();
        Task<Location?> GetByIdAsync(int id);
        Task CreateAsync(LocationDto dto);
        Task UpdateAsync(int id, LocationDto dto);
        Task DeleteAsync(int id);

        Task<List<LocationNearbyDto>> GetNearbyLocationsAsync(string address);
        Task UpdateCoordinatesAsync(int locationId);
        Task<List<LocationNearbyResponse>> GetLocationsNearbyAsync(string address, double radiusInKm, bool debug = false);

        Task UpdateCoordinatesByAddressAsync(int locationId);
        Task<List<NearbyCombinedItem>> GetNearbyCombinedAsync(string address, double radiusInKm, string? tags, int limit);
        Task<List<LocationNearbyResponse>> GetRegisteredLocationsNearbyAsync(double latitude, double longitude, double radiusKm = 50.0);

    }
}
