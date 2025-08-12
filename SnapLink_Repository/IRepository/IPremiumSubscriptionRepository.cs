using SnapLink_Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Repository.IRepository
{
    public interface IPremiumSubscriptionRepository
    {
        Task<PremiumSubscription?> GetActiveForPhotographerAsync(int photographerId);
        Task<PremiumSubscription?> GetActiveForLocationAsync(int locationId);
        Task<IEnumerable<PremiumSubscription>> GetByPhotographerAsync(int photographerId);
        Task<IEnumerable<PremiumSubscription>> GetByLocationAsync(int locationId);
        Task AddAsync(PremiumSubscription sub);
        Task UpdateAsync(PremiumSubscription sub);
        Task SaveChangesAsync();

        Task<Photographer?> GetPhotographerAsync(int photographerId);
        Task<Location?> GetLocationAsync(int locationId);
        Task<PremiumPackage?> GetPackageAsync(int packageId);
        Task<PremiumSubscription?> GetByIdAsync(int subscriptionId);
        Task<IEnumerable<PremiumSubscription>> GetToExpireAsync(DateTime asOfUtc);
    }
}
