using SnapLink_Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Repository.IRepository
{
    public interface IWalletRepository
    {
        Task<Wallet?> GetByUserIdAsync(int userId, bool forUpdate = false);
        Task UpdateAsync(Wallet wallet);
        Task SaveChangesAsync();
    }
}
