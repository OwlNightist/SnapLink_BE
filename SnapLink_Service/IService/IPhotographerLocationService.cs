using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService
{
    public interface IPhotographerLocationService
    {
        Task<IEnumerable<PhotographerListResponse>> GetPhotographersWithinRadiusAsync(double latitude, double longitude, double radiusKm);
        Task<double> CalculateDistanceToPhotographerAsync(double userLat, double userLon, int photographerId);
        Task<bool> ValidatePhotographerLocationAsync(int photographerId, double latitude, double longitude);
        Task UpdatePhotographerLocationAsync(int photographerId, string address, string googleMapsAddress, double? latitude, double? longitude);
    }
} 