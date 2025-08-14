using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SnapLink_Model.DTO.Request;
using SnapLink_Service.IService;

namespace SnapLink_Service.Service
{
    public class GooglePlacesService : IGooglePlacesService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GooglePlacesService> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://places.googleapis.com/v1/places:searchNearby";

        public GooglePlacesService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<GooglePlacesService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _apiKey = _configuration["GooglePlaces:ApiKey"] ?? throw new InvalidOperationException("Google Places API key not configured");
        }

        public async Task<string> SearchNearbyAsync(GooglePlacesNearbyRequest request)
        {
            try
            {
                _logger.LogInformation("Searching for nearby places at lat: {Latitude}, lng: {Longitude}, radius: {Radius}m", 
                    request.Latitude, request.Longitude, request.Radius);

                var requestBody = CreateRequestBody(request);
                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Set required headers
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("X-Goog-Api-Key", _apiKey);
                _httpClient.DefaultRequestHeaders.Add("X-Goog-FieldMask", request.FieldMask ?? "places.displayName,places.formattedAddress,places.location,places.types,places.photos,places.iconBackgroundColor,places.iconMaskBaseUri");

                var response = await _httpClient.PostAsync(_baseUrl, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Google Places API error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                    throw new HttpRequestException($"Google Places API error: {response.StatusCode} - {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received response from Google Places API with {Length} characters", responseContent.Length);
                
                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for nearby places");
                throw;
            }
        }

        public async Task<string> SearchNearbyAsync(double latitude, double longitude, double radius = 500.0, int maxResultCount = 20, List<string>? includedTypes = null)
        {
            var request = new GooglePlacesNearbyRequest
            {
                Latitude = latitude,
                Longitude = longitude,
                Radius = radius,
                MaxResultCount = maxResultCount,
                IncludedTypes = includedTypes
            };

            return await SearchNearbyAsync(request);
        }

        private object CreateRequestBody(GooglePlacesNearbyRequest request)
        {
            var body = new
            {
                maxResultCount = request.MaxResultCount,
                locationRestriction = new
                {
                    circle = new
                    {
                        center = new
                        {
                            latitude = request.Latitude,
                            longitude = request.Longitude
                        },
                        radius = request.Radius
                    }
                }
            };

            // Add includedTypes if specified
            if (request.IncludedTypes != null && request.IncludedTypes.Any())
            {
                var bodyWithTypes = new
                {
                    maxResultCount = request.MaxResultCount,
                    includedTypes = request.IncludedTypes,
                    locationRestriction = new
                    {
                        circle = new
                        {
                            center = new
                            {
                                latitude = request.Latitude,
                                longitude = request.Longitude
                            },
                            radius = request.Radius
                        }
                    }
                };
                return bodyWithTypes;
            }

            return body;
        }

        public async Task<string> GetPlacePhotoAsync(GooglePlacesPhotoRequest request)
        {
            try
            {
                            // Validate that at least one dimension is provided
            if (!request.MaxHeightPx.HasValue && !request.MaxWidthPx.HasValue)
            {
                throw new ArgumentException("At least one of MaxHeightPx or MaxWidthPx must be provided");
            }

                _logger.LogInformation("Getting place photo: {PhotoName}, MaxHeight: {MaxHeight}, MaxWidth: {MaxWidth}, SkipRedirect: {SkipRedirect}", 
                    request.PhotoName, request.MaxHeightPx, request.MaxWidthPx, request.SkipHttpRedirect);

                var photoUrl = BuildPhotoUrl(request);
                var response = await _httpClient.GetAsync(photoUrl);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Google Places Photo API error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                    throw new HttpRequestException($"Google Places Photo API error: {response.StatusCode} - {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received response from Google Places Photo API with {Length} characters", responseContent.Length);
                
                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting place photo");
                throw;
            }
        }

        private string BuildPhotoUrl(GooglePlacesPhotoRequest request)
        {
            var baseUrl = $"https://places.googleapis.com/v1/{request.PhotoName}/media";
            var queryParams = new List<string>();

            if (request.MaxHeightPx.HasValue)
            {
                queryParams.Add($"maxHeightPx={request.MaxHeightPx.Value}");
            }

            if (request.MaxWidthPx.HasValue)
            {
                queryParams.Add($"maxWidthPx={request.MaxWidthPx.Value}");
            }

            if (request.SkipHttpRedirect)
            {
                queryParams.Add("skipHttpRedirect=true");
            }

            queryParams.Add($"key={_apiKey}");

            var queryString = string.Join("&", queryParams);
            return $"{baseUrl}?{queryString}";
        }
    }
}
