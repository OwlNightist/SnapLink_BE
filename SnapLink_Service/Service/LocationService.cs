using AutoMapper;
using SnapLink_Model.DTO;
using SnapLink_Repository.Entity;
using SnapLink_Repository.IRepository;
using SnapLink_Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Service.Service
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _repo;
        private readonly IMapper _mapper;

        public LocationService(ILocationRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
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

        public async Task<IEnumerable<Location>> GetLocationsWithinRadiusAsync(double latitude, double longitude, double radiusKm)
        {
            // TODO: Add Latitude/Longitude fields to Location entity for real geospatial queries
            // For now, return an empty list to satisfy the interface
            return new List<Location>();
        }

      
        public Task<double> CalculateDistanceAsync(double lat1, double lon1, double lat2, double lon2)
        {
            var distance = CalculateDistance(lat1, lon1, lat2, lon2);
            return Task.FromResult(distance);
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Radius of the earth in km
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double deg)
        {
            return deg * (Math.PI / 180);
        }
    }
}
