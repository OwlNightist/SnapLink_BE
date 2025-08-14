using SnapLink_Model.DTO.Request;

namespace SnapLink_Service.IService
{
    public interface IGooglePlacesService
    {
        /// <summary>
        /// Search for nearby places using Google Places API
        /// </summary>
        /// <param name="request">Search parameters including latitude, longitude, radius, etc.</param>
        /// <returns>Raw Google Places API response as JSON string</returns>
        Task<string> SearchNearbyAsync(GooglePlacesNearbyRequest request);

        /// <summary>
        /// Search for nearby places with basic parameters
        /// </summary>
        /// <param name="latitude">Latitude coordinate</param>
        /// <param name="longitude">Longitude coordinate</param>
        /// <param name="radius">Search radius in meters</param>
        /// <param name="maxResultCount">Maximum number of results to return</param>
        /// <param name="includedTypes">Optional list of place types to include</param>
        /// <returns>Raw Google Places API response as JSON string</returns>
        Task<string> SearchNearbyAsync(double latitude, double longitude, double radius = 500.0, int maxResultCount = 20, List<string>? includedTypes = null);

        /// <summary>
        /// Get a place photo using Google Places Photo API
        /// </summary>
        /// <param name="request">Photo request parameters</param>
        /// <returns>Raw Google Places Photo API response as JSON string</returns>
        Task<string> GetPlacePhotoAsync(GooglePlacesPhotoRequest request);
    }
}
