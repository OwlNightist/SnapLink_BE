using SnapLink_Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Repository.IRepository
{
    public interface IPremiumPackageRepository
    {
        Task<PremiumPackage?> GetByIdAsync(int id);
        Task<IEnumerable<PremiumPackage>> GetAllAsync();
        Task AddAsync(PremiumPackage pkg);
        Task UpdateAsync(PremiumPackage pkg);
        Task DeleteAsync(PremiumPackage pkg);
        Task SaveChangesAsync();
    }
}
