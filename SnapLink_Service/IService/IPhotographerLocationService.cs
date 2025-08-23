using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService
{
    public interface IPhotographerLocationService
    {
        Task<IEnumerable<PhotographerListResponse>> GetPhotographersWithinRadiusAsync(double latitude, double longitude, double radiusKm);
        Task<double> CalculateDistanceToPhotographerAsync(double userLat, double userLon, int photographerId);
        Task<bool> ValidatePhotographerLocationAsync(int photographerId, double latitude, double longitude);
        Task UpdatePhotographerLocationAsync(int photographerId, string address, string googleMapsAddress, double? latitude, double? longitude);
        Task<IEnumerable<PhotographerListResponse>> GetRecommendedPhotographersAsync(double latitude, double longitude, int userId, int? locationId = null, double radiusKm = 10.0, int maxResults = 20);
        Task<IEnumerable<PhotographerListResponse>> GetPhotographersByUserStylesAsync(int userId, double userLatitude, double userLongitude);
        Task<IEnumerable<PhotographerListResponse>> GetPopularPhotographersAsync(double? latitude = null, double? longitude = null, int page = 1, int pageSize = 10);
    }
} 