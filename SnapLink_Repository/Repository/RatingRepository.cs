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
    public class RatingRepository : IRatingRepository
    {
        private readonly SnaplinkDbContext _ctx;
        public RatingRepository(SnaplinkDbContext ctx) => _ctx = ctx;

        public async Task<Rating?> GetByIdAsync(int id) =>
            await _ctx.Set<Rating>()
                .Include(r => r.Booking)
                .Include(r => r.Photographer)
                .Include(r => r.Location)
                .FirstOrDefaultAsync(r => r.RatingId == id);

        public async Task<IEnumerable<Rating>> GetAllAsync() =>
            await _ctx.Set<Rating>()
                .AsNoTracking()
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<Rating>> GetByPhotographerAsync(int photographerId) =>
            await _ctx.Set<Rating>()
                .AsNoTracking()
                .Where(r => r.PhotographerId == photographerId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<Rating>> GetByLocationAsync(int locationId) =>
            await _ctx.Set<Rating>()
                .AsNoTracking()
                .Where(r => r.LocationId == locationId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<Rating?> GetExistingAsync(int bookingId, int reviewerUserId, int? photographerId, int? locationId) =>
            await _ctx.Set<Rating>()
                .FirstOrDefaultAsync(r =>
                    r.BookingId == bookingId &&
                    r.ReviewerUserId == reviewerUserId &&
                    r.PhotographerId == photographerId &&
                    r.LocationId == locationId);

        public async Task AddAsync(Rating rating) => await _ctx.Set<Rating>().AddAsync(rating);
        public Task UpdateAsync(Rating rating) { _ctx.Set<Rating>().Update(rating); return Task.CompletedTask; }
        public Task DeleteAsync(Rating rating) { _ctx.Set<Rating>().Remove(rating); return Task.CompletedTask; }
        public async Task SaveChangesAsync() => await _ctx.SaveChangesAsync();

        public async Task<Booking?> GetBookingAsync(int bookingId) =>
            await _ctx.Bookings.AsNoTracking().FirstOrDefaultAsync(b => b.BookingId == bookingId);

        public async Task<Photographer?> GetPhotographerAsync(int photographerId) =>
            await _ctx.Photographers.FirstOrDefaultAsync(p => p.PhotographerId == photographerId);

        public async Task<Location?> GetLocationAsync(int locationId) =>
            await _ctx.Locations.FirstOrDefaultAsync(l => l.LocationId == locationId);
    }
}
