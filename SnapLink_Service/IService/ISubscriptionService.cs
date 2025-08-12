using SnapLink_Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Service.IService
{
    public interface ISubscriptionService
    {
        Task<SubscriptionDto> SubscribeAsync(SubscribePackageDto dto);
        Task<IEnumerable<SubscriptionDto>> GetByPhotographerAsync(int photographerId);
        Task<IEnumerable<SubscriptionDto>> GetByLocationAsync(int locationId);
        Task<string> CancelAsync(int subscriptionId, string? reason = null);
        Task<int> ExpireOverduesAsync();
    }
}
