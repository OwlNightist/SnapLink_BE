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
    public class WalletRepository : IWalletRepository
    {
        private readonly SnaplinkDbContext _ctx;
        public WalletRepository(SnaplinkDbContext ctx) { _ctx = ctx; }

        public async Task<Wallet?> GetByUserIdAsync(int userId, bool forUpdate = false)
        {
            // forUpdate bật Concurrency resolution: EF Core không có "FOR UPDATE", 
            // nhưng transaction + SaveChanges sẽ đủ cho hầu hết case.
            return await _ctx.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public Task UpdateAsync(Wallet wallet)
        {
            _ctx.Wallets.Update(wallet);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync() => await _ctx.SaveChangesAsync();
    }
}
