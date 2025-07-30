using SnapLink_Repository.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SnapLink_Repository.IRepository
{
    public interface IDeviceInfoRepository
    {
        // Core operations
        Task<DeviceInfo?> GetDeviceInfoByIdAsync(int deviceInfoId);
        Task<IEnumerable<DeviceInfo>> GetDeviceInfosByPhotographerIdAsync(int photographerId);
        Task<IEnumerable<DeviceInfo>> GetAllDeviceInfosAsync();
        Task AddDeviceInfoAsync(DeviceInfo deviceInfo);
        Task UpdateDeviceInfoAsync(DeviceInfo deviceInfo);
        Task DeleteDeviceInfoAsync(DeviceInfo deviceInfo);
        Task SaveChangesAsync();

        // Search and filter operations
        Task<IEnumerable<DeviceInfo>> GetDeviceInfosByTypeAsync(string deviceType);
        Task<IEnumerable<DeviceInfo>> GetDeviceInfosByBrandAsync(string brand);
        Task<IEnumerable<DeviceInfo>> GetDeviceInfosByStatusAsync(string status);
        Task<IEnumerable<DeviceInfo>> GetActiveDeviceInfosAsync();
        Task<IEnumerable<DeviceInfo>> GetRecentlyUsedDevicesAsync(DateTime fromDate);

        // Analytics operations
        Task<int> GetTotalDeviceCountAsync();
        Task<int> GetActiveDeviceCountAsync();
        Task<Dictionary<string, int>> GetDeviceTypeDistributionAsync();
        Task<Dictionary<string, int>> GetBrandDistributionAsync();
        Task<Dictionary<string, int>> GetCameraResolutionDistributionAsync();
        Task<int> GetDevicesUsedInLastWeekAsync();
        Task<int> GetDevicesUsedInLastMonthAsync();

        // Advanced search
        Task<IEnumerable<DeviceInfo>> SearchDeviceInfosAsync(
            int? photographerId = null,
            string? deviceType = null,
            string? brand = null,
            string? status = null,
            DateTime? lastUsedFrom = null,
            DateTime? lastUsedTo = null,
            int page = 1,
            int pageSize = 10);
    }
} 