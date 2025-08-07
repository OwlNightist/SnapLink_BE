using AutoMapper;
using SnapLink_Model.DTO;
using SnapLink_Model.DTO.Response;
using SnapLink_Repository.Entity;
using SnapLink_Repository.IRepository;
using SnapLink_Repository.Repository;
using SnapLink_Service.IService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace SnapLink_Service.Service
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _repo;
        private readonly IMapper _mapper;
        private readonly IGeoProvider _geo;
        private readonly HttpClient _httpClient;
        private readonly INearbyPoiProvider _poi;


        public LocationService(HttpClient httpClient,ILocationRepository repo, IMapper mapper, IGeoProvider geo, INearbyPoiProvider poi)
        {
            _repo = repo;
            _mapper = mapper;
            _httpClient = httpClient;
            _geo = geo;
            _poi = poi;
        }

        public async Task<IEnumerable<Location>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<Location?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

        public async Task CreateAsync(LocationDto dto)
        {
            var location = new Location
            {
                LocationOwnerId = dto.LocationOwnerId,
                Name = dto.Name,
                Address = dto.Address,
                Description = dto.Description,
                Amenities = dto.Amenities,
                HourlyRate = dto.HourlyRate,
                Capacity = dto.Capacity,
                Indoor = dto.Indoor,
                Outdoor = dto.Outdoor,
                AvailabilityStatus = dto.AvailabilityStatus,
                FeaturedStatus = dto.FeaturedStatus,
                VerificationStatus = dto.VerificationStatus,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(location);
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, LocationDto dto)
        {
            var location = await _repo.GetByIdAsync(id);
            if (location == null) throw new Exception("Location not found");

            location.Name = dto.Name;
            location.Address = dto.Address;
            location.Description = dto.Description;
            location.Amenities = dto.Amenities;
            location.HourlyRate = dto.HourlyRate;
            location.Capacity = dto.Capacity;
            location.Indoor = dto.Indoor;
            location.Outdoor = dto.Outdoor;
            location.AvailabilityStatus = dto.AvailabilityStatus;
            location.FeaturedStatus = dto.FeaturedStatus;
            location.VerificationStatus = dto.VerificationStatus;
            location.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(location);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var location = await _repo.GetByIdAsync(id);
            if (location == null) throw new Exception("Location not found");
            await _repo.DeleteAsync(location);
            await _repo.SaveChangesAsync();
        }

        public async Task<List<LocationNearbyDto>> GetNearbyLocationsAsync(string address)
        {
            var (latitude, longitude) = await GetLatLongFromAddress(address);
            var locations = await _repo.GetAllLocationsAsync();
            return locations
            .Select(loc => new LocationNearbyDto
            {
                LocationId = loc.LocationId,
                Name = loc.Name,
                Address = loc.Address,
                DistanceInKm = GetDistance(latitude, longitude, (double)loc.Latitude!, (double)loc.Longitude!)
            })
            .OrderBy(x => x.DistanceInKm)
            .ToList();
        }
        public async Task UpdateCoordinatesAsync(int locationId)
        {
            var location = await _repo.GetByIdAsync(locationId);
            if (location == null) throw new Exception("Location not found.");
            if (string.IsNullOrWhiteSpace(location.Address)) throw new Exception("Address is empty.");

            var (lat, lon) = await GetLatLongFromAddress(location.Address);
            location.Latitude = lat;
            location.Longitude = lon;
            location.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(location);
            await _repo.SaveChangesAsync();
        }
        private async Task<(double lat, double lon)> GetLatLongFromAddress(string address)
        {
            string encoded = HttpUtility.UrlEncode(address);
            string url = $"https://nominatim.openstreetmap.org/search?q={encoded}&format=json&limit=1";

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<List<NominatimResponse>>(content);
            if (result == null || result.Count == 0)
                throw new Exception("Không tìm thấy tọa độ cho địa chỉ.");

            return (double.Parse(result[0].Lat), double.Parse(result[0].Lon));
        }
        private class NominatimResponse
        {
            public string Lat { get; set; } = "";
            public string Lon { get; set; } = "";
        }

        private double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // km
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }



        private double DegreesToRadians(double deg) => deg * (Math.PI / 180);


        public async Task<List<LocationNearbyResponse>> GetLocationsNearbyAsync(string address, double radiusInKm, bool debug = false)
        {
            var coords = await _geo.GeocodeAsync(address);
            if (coords == null) return new List<LocationNearbyResponse>();

            var (lat, lon) = coords.Value;
            var all = await _repo.GetAllAsyncc();

            var list = all
                .Where(l => l.Latitude.HasValue && l.Longitude.HasValue)
                .Select(l =>
                {
                    var d = Haversine(lat, lon, l.Latitude!.Value, l.Longitude!.Value);
                    return new LocationNearbyResponse
                    {
                        LocationId = l.LocationId,
                        Name = l.Name,
                        Address = l.Address,
                        Latitude = l.Latitude,
                        Longitude = l.Longitude,
                        DistanceInKm = Math.Round(d, 2)
                    };
                });
                /*.Where(x => x.DistanceInKm <= radiusInKm)
                .OrderBy(x => x.DistanceInKm)
                .ToList();

            return list;*/
                if (!debug)
                list = list.Where(x => x.DistanceInKm <= radiusInKm);

            return list.OrderBy(x => x.DistanceInKm).ToList();
        }

        public async Task UpdateCoordinatesByAddressAsync(int locationId)
        {
            var loc = await _repo.GetByIdAsyncc(locationId)
                      ?? throw new Exception("Location not found");

            if (string.IsNullOrWhiteSpace(loc.Address))
                throw new Exception("Location has no address to geocode");

            var coords = await _geo.GeocodeAsync(loc.Address);
            if (coords == null) throw new Exception("Cannot geocode this address");

            loc.Latitude = coords.Value.lat;
            loc.Longitude = coords.Value.lon;
            loc.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsyncc(loc);
            await _repo.SaveChangesAsync();
        }

        // Haversine (km)
        private static double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLon = (lon2 - lon1) * Math.PI / 180;
            lat1 *= Math.PI / 180; lat2 *= Math.PI / 180;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            return 2 * R * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }


        public async Task<List<NearbyCombinedItem>> GetNearbyCombinedAsync(string address, double radiusInKm, string? tags, int limit)
        {
            var coords = await _geo.GeocodeAsync(address);
            if (coords == null) return new List<NearbyCombinedItem>();
            var (lat, lon) = coords.Value;

            var radiusMeters = (int)Math.Round(radiusInKm * 1000);

            // INTERNAL (DB)
            var db = await _repo.GetAllAsyncc();
            var internals = db
                .Where(l => l.Latitude.HasValue && l.Longitude.HasValue)
                .Select(l => new NearbyCombinedItem
                {
                    Source = "internal",
                    LocationId = l.LocationId,
                    Name = l.Name,
                    Address = l.Address,
                    Latitude = l.Latitude!.Value,
                    Longitude = l.Longitude!.Value,
                    DistanceInKm = Math.Round(HaversineKm(lat, lon, l.Latitude!.Value, l.Longitude!.Value), 2)
                })
                .Where(x => x.DistanceInKm <= radiusInKm)
                .ToList();

            // EXTERNAL (LocationIQ Nearby)
            var externals = await _poi.GetNearbyAsync(lat, lon, radiusMeters, tags, limit);
            foreach (var e in externals)
                e.DistanceInKm = Math.Round(HaversineKm(lat, lon, e.Latitude, e.Longitude), 2);

            // Gộp + sort
            var combined = internals.Concat(externals)
                                    .OrderBy(x => x.DistanceInKm)
                                    .ToList();

            // Tuỳ chọn: bỏ trùng (theo khoảng cách rất gần)
            // combined = DeduplicateByProximity(combined);

            return combined;
        }

        private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            double dLat = (lat2 - lat1) * Math.PI / 180d;
            double dLon = (lon2 - lon1) * Math.PI / 180d;
            lat1 *= Math.PI / 180d; lat2 *= Math.PI / 180d;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            return 2 * R * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }
    }
}
