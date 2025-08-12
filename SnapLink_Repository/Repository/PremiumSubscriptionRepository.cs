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
    public class PremiumSubscriptionRepository : IPremiumSubscriptionRepository
    {
        private readonly SnaplinkDbContext _ctx;
        public PremiumSubscriptionRepository(SnaplinkDbContext ctx) => _ctx = ctx;

        public async Task<PremiumSubscription?> GetActiveForPhotographerAsync(int photographerId) =>
            await _ctx.PremiumSubscriptions
                .Where(s => s.Status == "Active" && s.PhotographerId == photographerId && s.EndDate >= DateTime.UtcNow)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync();

        public async Task<PremiumSubscription?> GetActiveForLocationAsync(int locationId) =>
            await _ctx.PremiumSubscriptions
                .Where(s => s.Status == "Active" && s.LocationId == locationId && s.EndDate >= DateTime.UtcNow)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync();

        public async Task<IEnumerable<PremiumSubscription>> GetByPhotographerAsync(int photographerId) =>
            await _ctx.PremiumSubscriptions
                .Include(s => s.Package)
                .Where(s => s.PhotographerId == photographerId)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();

        public async Task<IEnumerable<PremiumSubscription>> GetByLocationAsync(int locationId) =>
            await _ctx.PremiumSubscriptions
                .Include(s => s.Package)
                .Where(s => s.LocationId == locationId)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();

        public async Task AddAsync(PremiumSubscription sub) => await _ctx.PremiumSubscriptions.AddAsync(sub);
        public Task UpdateAsync(PremiumSubscription sub) { _ctx.PremiumSubscriptions.Update(sub); return Task.CompletedTask; }
        public async Task SaveChangesAsync() => await _ctx.SaveChangesAsync();

        public async Task<Photographer?> GetPhotographerAsync(int photographerId) =>
            await _ctx.Photographers.FirstOrDefaultAsync(p => p.PhotographerId == photographerId);

        public async Task<Location?> GetLocationAsync(int locationId) =>
            await _ctx.Locations
        .Include(l => l.LocationOwner) // <<< để có l.LocationOwner.UserId
        .FirstOrDefaultAsync(l => l.LocationId == locationId);

        public async Task<PremiumPackage?> GetPackageAsync(int packageId) =>
            await _ctx.PremiumPackages.FirstOrDefaultAsync(p => p.PackageId == packageId);
        public async Task<PremiumSubscription?> GetByIdAsync(int subscriptionId) =>
            await _ctx.PremiumSubscriptions
        .Include(s => s.Package)
        .FirstOrDefaultAsync(s => s.PremiumSubscriptionId == subscriptionId);

        public async Task<IEnumerable<PremiumSubscription>> GetToExpireAsync(DateTime asOfUtc) =>
            await _ctx.PremiumSubscriptions
                .Where(s => s.Status == "Active" && s.EndDate < asOfUtc)
                .ToListAsync();
        public async Task<IList<PremiumSubscription>> GetAllAsync(string? status = "Active")
        {
            var q = _ctx.PremiumSubscriptions
                .Include(s => s.Package)
                .Include(s => s.Photographer).ThenInclude(p => p.User)
                .Include(s => s.Location).ThenInclude(l => l.LocationOwner).ThenInclude(o => o.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                q = q.Where(s => s.Status == status);

            return await q.OrderByDescending(s => s.StartDate).ToListAsync();
        }

        public async Task<IList<PremiumSubscription>> GetAllPhotographerAsync(string? status = "Active")
        {
            var q = _ctx.PremiumSubscriptions
                .Include(s => s.Package)
                .Include(s => s.Photographer).ThenInclude(p => p.User)
                .Where(s => s.PhotographerId != null);

            if (!string.IsNullOrWhiteSpace(status))
                q = q.Where(s => s.Status == status);

            return await q.OrderByDescending(s => s.StartDate).ToListAsync();
        }

        public async Task<IList<PremiumSubscription>> GetAllLocationAsync(string? status = "Active")
        {
            var q = _ctx.PremiumSubscriptions
                .Include(s => s.Package)
                .Include(s => s.Location).ThenInclude(l => l.LocationOwner).ThenInclude(o => o.User)
                .Where(s => s.LocationId != null);

            if (!string.IsNullOrWhiteSpace(status))
                q = q.Where(s => s.Status == status);

            return await q.OrderByDescending(s => s.StartDate).ToListAsync();
        }
    }
}
