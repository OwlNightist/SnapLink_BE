using SnapLink_Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Repository.Repository
{
    public interface IRoleRepository
    {
        Task<bool> AnyRoleExistsAsync();
        Task AddRolesAsync(IEnumerable<Role> roles);
        Task SaveChangesAsync();
    }
}
