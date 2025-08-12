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
    public class TransactionRepository : ITransactionRepository
    {
        private readonly SnaplinkDbContext _ctx;
        public TransactionRepository(SnaplinkDbContext ctx) { _ctx = ctx; }

        public async Task AddAsync(Transaction tx) => await _ctx.Transactions.AddAsync(tx);
        public async Task SaveChangesAsync() => await _ctx.SaveChangesAsync();
    }
}
