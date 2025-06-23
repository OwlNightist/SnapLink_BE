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

        public LocationService(ILocationRepository repo)
        {
            _repo = repo;
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
    }
}
