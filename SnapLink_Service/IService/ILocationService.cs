using SnapLink_Model.DTO;
using SnapLink_Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Service.IService
{
    public interface ILocationService
    {
        Task<IEnumerable<Location>> GetAllAsync();
        Task<Location?> GetByIdAsync(int id);
        Task CreateAsync(LocationDto dto);
        Task UpdateAsync(int id, LocationDto dto);
        Task DeleteAsync(int id);
    }
}
