using SnapLink_Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Service.IService
{
    public interface IPackageService
    {
        Task<int> CreateAsync(CreatePackageDto dto);
        Task UpdateAsync(int packageId, UpdatePackageDto dto);
        Task DeleteAsync(int packageId);
        Task<PackageDto?> GetByIdAsync(int packageId);
        Task<IEnumerable<PackageDto>> GetAllAsync();
    }
}
