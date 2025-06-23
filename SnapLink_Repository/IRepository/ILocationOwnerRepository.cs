using SnapLink_Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Repository.IRepository
{
    public interface ILocationOwnerRepository
    {
        Task<IEnumerable<LocationOwner>> GetAllAsync();
        Task<LocationOwner?> GetByIdAsync(int id);
        Task AddAsync(LocationOwner owner);
        Task UpdateAsync(LocationOwner owner);
        Task DeleteAsync(LocationOwner owner);
        Task SaveChangesAsync();
    }
}
