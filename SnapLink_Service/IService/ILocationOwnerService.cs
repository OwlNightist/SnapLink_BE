using SnapLink_Model.DTO;
using SnapLink_Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Service.IService
{
    public interface ILocationOwnerService
    {
        Task<IEnumerable<LocationOwner>> GetAllAsync();
        Task<LocationOwner?> GetByIdAsync(int id);
        Task CreateAsync(LocationOwnerDto dto);
        Task UpdateAsync(int id, LocationOwnerDto dto);
        Task DeleteAsync(int id);
    }
}
