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
    public class LocationRepository : ILocationRepository
    {
        private readonly SnaplinkDbContext _context;

        public LocationRepository(SnaplinkDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Location>> GetAllAsync() =>
            await _context.Locations.Include(l => l.LocationOwner).ToListAsync();

        public async Task<Location?> GetByIdAsync(int id) =>
            await _context.Locations.Include(l => l.LocationOwner).FirstOrDefaultAsync(l => l.LocationId == id);

        public async Task AddAsync(Location location) => await _context.Locations.AddAsync(location);

        public Task UpdateAsync(Location location)
        {
            _context.Locations.Update(location);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Location location)
        {
            _context.Locations.Remove(location);
            return Task.CompletedTask;
        }
        public async Task<List<Location>> GetAllLocationsAsync()
        {
            return await _context.Locations
                .Where(l => l.Latitude != null && l.Longitude != null)
                .ToListAsync();
        }

        /* public async Task<List<Location>> GetAllAsyncc() =>
        await _context.Locations
                   .Where(l => l.Latitude != null && l.Longitude != null)
                   .ToListAsync();*/
        public async Task<List<Location>> GetAllAsyncc()
     => await _context.Locations.ToListAsync();
        public async Task<Location?> GetByIdAsyncc(int id) =>
            await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == id);

        public Task UpdateAsyncc(Location location)
        {
            _context.Locations.Update(location);
            return Task.CompletedTask;
        }
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
