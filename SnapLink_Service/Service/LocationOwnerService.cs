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
    public class LocationOwnerService : ILocationOwnerService
    {
        private readonly ILocationOwnerRepository _repo;

        public LocationOwnerService(ILocationOwnerRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<LocationOwner>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<LocationOwner?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

        public async Task CreateAsync(LocationOwnerDto dto)
        {
            var owner = new LocationOwner
            {
                UserId = dto.UserId,
                BusinessName = dto.BusinessName,
                BusinessAddress = dto.BusinessAddress,
                BusinessRegistrationNumber = dto.BusinessRegistrationNumber,
                VerificationStatus = dto.VerificationStatus
            };
            await _repo.AddAsync(owner);
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, LocationOwnerDto dto)
        {
            var owner = await _repo.GetByIdAsync(id);
            if (owner == null) throw new Exception("Not found");

            owner.BusinessName = dto.BusinessName;
            owner.BusinessAddress = dto.BusinessAddress;
            owner.BusinessRegistrationNumber = dto.BusinessRegistrationNumber;
            owner.VerificationStatus = dto.VerificationStatus;

            await _repo.UpdateAsync(owner);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var owner = await _repo.GetByIdAsync(id);
            if (owner == null) throw new Exception("Not found");
            await _repo.DeleteAsync(owner);
            await _repo.SaveChangesAsync();
        }
    }
}
