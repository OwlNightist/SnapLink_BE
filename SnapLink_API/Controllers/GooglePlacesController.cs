using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO.Request;
using SnapLink_Service.IService;

namespace SnapLink_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GooglePlacesController : ControllerBase
    {
        private readonly IGooglePlacesService _googlePlacesService;
        private readonly ILogger<GooglePlacesController> _logger;

        public GooglePlacesController(
            IGooglePlacesService googlePlacesService,
            ILogger<GooglePlacesController> logger)
        {
            _googlePlacesService = googlePlacesService;
            _logger = logger;
        }

        // /// <summary>
        // /// Search for nearby places using Google Places API
        // /// </summary>
        // /// <param name="request">Search parameters</param>
        // /// <returns>Raw Google Places API response</returns>
        // [HttpPost("search-nearby")]
        // public async Task<ActionResult<string>> SearchNearby([FromBody] GooglePlacesNearbyRequest request)
        // {
        //     try
        //     {
        //         if (!ModelState.IsValid)
        //         {
        //             return BadRequest(ModelState);
        //         }

        //         var result = await _googlePlacesService.SearchNearbyAsync(request);
        //         return Content(result, "application/json");
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error in SearchNearby endpoint");
        //         return StatusCode(500, new { error = "Internal server error occurred while searching for nearby places" });
        //     }
        // }

        /// <summary>
        /// Search for nearby places with basic parameters
        /// </summary>
        /// <param name="latitude">Latitude coordinate</param>
        /// <param name="longitude">Longitude coordinate</param>
        /// <param name="radius">Search radius in meters (default: 500)</param>
        /// <param name="maxResultCount">Maximum number of results (default: 20, max: 100)</param>
        /// <param name="includedTypes">Optional list of place types to include</param>
        /// <returns>Raw Google Places API response</returns>
        [HttpGet("search-nearby")]
        public async Task<ActionResult<string>> SearchNearbyGet(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radius = 500.0,
            [FromQuery] int maxResultCount = 20,
            [FromQuery] List<string>? includedTypes = null)
        {
            try
            {
                // Validate parameters
                if (latitude < -90 || latitude > 90)
                    return BadRequest("Latitude must be between -90 and 90");
                
                if (longitude < -180 || longitude > 180)
                    return BadRequest("Longitude must be between -180 and 180");
                
                if (radius < 1 || radius > 50000)
                    return BadRequest("Radius must be between 1 and 50000 meters");
                
                if (maxResultCount < 1 || maxResultCount > 100)
                    return BadRequest("MaxResultCount must be between 1 and 100");

                var result = await _googlePlacesService.SearchNearbyAsync(latitude, longitude, radius, maxResultCount, includedTypes);
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SearchNearbyGet endpoint");
                return StatusCode(500, new { error = "Internal server error occurred while searching for nearby places" });
            }
        }

        /// <summary>
        /// Get available place types for filtering
        /// </summary>
        /// <returns>List of available place types</returns>
        [HttpGet("place-types")]
        public ActionResult<List<string>> GetPlaceTypes()
        {
            var placeTypes = new List<string>
            {
                "restaurant", "cafe", "bar", "bakery", "food",
                "lodging", "hotel", "motel", "resort",
                "shopping_mall", "store", "supermarket", "pharmacy",
                "gas_station", "parking", "atm", "bank",
                "hospital", "pharmacy", "dentist", "veterinary_care",
                "school", "university", "library", "museum",
                "park", "amusement_park", "aquarium", "zoo",
                "movie_theater", "theater", "stadium", "gym",
                "beauty_salon", "spa", "hair_care", "car_wash",
                "police", "fire_station", "post_office", "embassy",
                "church", "mosque", "synagogue", "hindu_temple",
                "cemetery", "funeral_home", "real_estate_agency",
                "travel_agency", "insurance_agency", "lawyer",
                "accounting", "dentist", "doctor", "hospital",
                "pharmacy", "veterinary_care", "car_rental",
                "car_dealer", "car_repair", "car_wash",
                "gas_station", "parking", "atm", "bank",
                "post_office", "police", "fire_station"
            };

            return Ok(placeTypes);
        }

        /// <summary>
        /// Get a place photo using Google Places Photo API
        /// </summary>
        /// <param name="request">Photo request parameters</param>
        /// <returns>Raw Google Places Photo API response</returns>
        [HttpPost("photo")]
        public async Task<ActionResult<string>> GetPlacePhoto([FromBody] GooglePlacesPhotoRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate that at least one dimension is provided
                if (!request.MaxHeightPx.HasValue && !request.MaxWidthPx.HasValue)
                {
                    return BadRequest("At least one of MaxHeightPx or MaxWidthPx must be provided");
                }

                var result = await _googlePlacesService.GetPlacePhotoAsync(request);
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetPlacePhoto endpoint");
                return StatusCode(500, new { error = "Internal server error occurred while getting place photo" });
            }
        }
    }
}
