using Microsoft.EntityFrameworkCore;
using SnapLink_Repository.DBContext;
using SnapLink_Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Repository.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly SnaplinkDbContext _context;
        public RoleRepository(SnaplinkDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AnyRoleExistsAsync()
        {
            return await _context.Roles.AnyAsync();
        }

        public async Task AddRolesAsync(IEnumerable<Role> roles)
        {
            await _context.Roles.AddRangeAsync(roles);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


    }
}
