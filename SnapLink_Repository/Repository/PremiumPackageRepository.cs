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
    public class PremiumPackageRepository : IPremiumPackageRepository
    {
        private readonly SnaplinkDbContext _ctx;
        public PremiumPackageRepository(SnaplinkDbContext ctx) => _ctx = ctx;

        public async Task<PremiumPackage?> GetByIdAsync(int id) =>
            await _ctx.PremiumPackages.FirstOrDefaultAsync(p => p.PackageId == id);

        public async Task<IEnumerable<PremiumPackage>> GetAllAsync() =>
            await _ctx.PremiumPackages.AsNoTracking().ToListAsync();

        public async Task AddAsync(PremiumPackage pkg) => await _ctx.PremiumPackages.AddAsync(pkg);
        public Task UpdateAsync(PremiumPackage pkg) { _ctx.PremiumPackages.Update(pkg); return Task.CompletedTask; }
        public Task DeleteAsync(PremiumPackage pkg) { _ctx.PremiumPackages.Remove(pkg); return Task.CompletedTask; }
        public async Task SaveChangesAsync() => await _ctx.SaveChangesAsync();
    }
}
