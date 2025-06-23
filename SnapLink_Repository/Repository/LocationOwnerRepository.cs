using Microsoft.EntityFrameworkCore;
using SnapLink_Repository.DBContext;
using SnapLink_Repository.Entity;
using SnapLink_Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Repository.Repository
{
    public class LocationOwnerRepository : ILocationOwnerRepository
    {
        private readonly SnaplinkDbContext _context;

        public LocationOwnerRepository(SnaplinkDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LocationOwner>> GetAllAsync() =>
            await _context.LocationOwners.Include(lo => lo.User).ToListAsync();

        public async Task<LocationOwner?> GetByIdAsync(int id) =>
            await _context.LocationOwners.Include(lo => lo.User).FirstOrDefaultAsync(lo => lo.LocationOwnerId == id);

        public async Task AddAsync(LocationOwner owner) => await _context.LocationOwners.AddAsync(owner);

        public Task UpdateAsync(LocationOwner owner)
        {
            _context.LocationOwners.Update(owner);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(LocationOwner owner)
        {
            _context.LocationOwners.Remove(owner);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
