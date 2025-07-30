using Microsoft.EntityFrameworkCore;
using SnapLink_Repository.DBContext;
using SnapLink_Repository.Entity;
using SnapLink_Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnapLink_Repository.Repository
{
    public class DeviceInfoRepository : IDeviceInfoRepository
    {
        private readonly SnaplinkDbContext _context;

        public DeviceInfoRepository(SnaplinkDbContext context)
        {
            _context = context;
        }

        public async Task<DeviceInfo?> GetDeviceInfoByIdAsync(int deviceInfoId)
        {
            return await _context.DeviceInfos
                .Include(d => d.Photographer)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(d => d.DeviceInfoId == deviceInfoId);
        }

        public async Task<IEnumerable<DeviceInfo>> GetDeviceInfosByPhotographerIdAsync(int photographerId)
        {
            return await _context.DeviceInfos
                .Include(d => d.Photographer)
                .ThenInclude(p => p.User)
                .Where(d => d.PhotographerId == photographerId)
                .OrderByDescending(d => d.LastUsedAt)
                .ThenByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DeviceInfo>> GetAllDeviceInfosAsync()
        {
            return await _context.DeviceInfos
                .Include(d => d.Photographer)
                .ThenInclude(p => p.User)
                .OrderByDescending(d => d.LastUsedAt)
                .ThenByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task AddDeviceInfoAsync(DeviceInfo deviceInfo)
        {
            deviceInfo.CreatedAt = DateTime.UtcNow;
            await _context.DeviceInfos.AddAsync(deviceInfo);
        }

        public async Task UpdateDeviceInfoAsync(DeviceInfo deviceInfo)
        {
            deviceInfo.UpdatedAt = DateTime.UtcNow;
            _context.DeviceInfos.Update(deviceInfo);
        }

        public async Task DeleteDeviceInfoAsync(DeviceInfo deviceInfo)
        {
            _context.DeviceInfos.Remove(deviceInfo);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<DeviceInfo>> GetDeviceInfosByTypeAsync(string deviceType)
        {
            return await _context.DeviceInfos
                .Include(d => d.Photographer)
                .ThenInclude(p => p.User)
                .Where(d => d.DeviceType == deviceType)
                .OrderByDescending(d => d.LastUsedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DeviceInfo>> GetDeviceInfosByBrandAsync(string brand)
        {
            return await _context.DeviceInfos
                .Include(d => d.Photographer)
                .ThenInclude(p => p.User)
                .Where(d => d.Brand == brand)
                .OrderByDescending(d => d.LastUsedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DeviceInfo>> GetDeviceInfosByStatusAsync(string status)
        {
            return await _context.DeviceInfos
                .Include(d => d.Photographer)
                .ThenInclude(p => p.User)
                .Where(d => d.Status == status)
                .OrderByDescending(d => d.LastUsedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DeviceInfo>> GetActiveDeviceInfosAsync()
        {
            return await _context.DeviceInfos
                .Include(d => d.Photographer)
                .ThenInclude(p => p.User)
                .Where(d => d.Status == "Active")
                .OrderByDescending(d => d.LastUsedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DeviceInfo>> GetRecentlyUsedDevicesAsync(DateTime fromDate)
        {
            return await _context.DeviceInfos
                .Include(d => d.Photographer)
                .ThenInclude(p => p.User)
                .Where(d => d.LastUsedAt >= fromDate)
                .OrderByDescending(d => d.LastUsedAt)
                .ToListAsync();
        }

        public async Task<int> GetTotalDeviceCountAsync()
        {
            return await _context.DeviceInfos.CountAsync();
        }

        public async Task<int> GetActiveDeviceCountAsync()
        {
            return await _context.DeviceInfos.CountAsync(d => d.Status == "Active");
        }

        public async Task<Dictionary<string, int>> GetDeviceTypeDistributionAsync()
        {
            return await _context.DeviceInfos
                .GroupBy(d => d.DeviceType)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<Dictionary<string, int>> GetBrandDistributionAsync()
        {
            return await _context.DeviceInfos
                .GroupBy(d => d.Brand)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<Dictionary<string, int>> GetCameraResolutionDistributionAsync()
        {
            return await _context.DeviceInfos
                .Where(d => !string.IsNullOrEmpty(d.CameraResolution))
                .GroupBy(d => d.CameraResolution)
                .ToDictionaryAsync(g => g.Key!, g => g.Count());
        }

        public async Task<int> GetDevicesUsedInLastWeekAsync()
        {
            var lastWeek = DateTime.UtcNow.AddDays(-7);
            return await _context.DeviceInfos.CountAsync(d => d.LastUsedAt >= lastWeek);
        }

        public async Task<int> GetDevicesUsedInLastMonthAsync()
        {
            var lastMonth = DateTime.UtcNow.AddMonths(-1);
            return await _context.DeviceInfos.CountAsync(d => d.LastUsedAt >= lastMonth);
        }

        public async Task<IEnumerable<DeviceInfo>> SearchDeviceInfosAsync(
            int? photographerId = null,
            string? deviceType = null,
            string? brand = null,
            string? status = null,
            DateTime? lastUsedFrom = null,
            DateTime? lastUsedTo = null,
            int page = 1,
            int pageSize = 10)
        {
            var query = _context.DeviceInfos
                .Include(d => d.Photographer)
                .ThenInclude(p => p.User)
                .AsQueryable();

            if (photographerId.HasValue)
                query = query.Where(d => d.PhotographerId == photographerId.Value);

            if (!string.IsNullOrEmpty(deviceType))
                query = query.Where(d => d.DeviceType == deviceType);

            if (!string.IsNullOrEmpty(brand))
                query = query.Where(d => d.Brand == brand);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(d => d.Status == status);

            if (lastUsedFrom.HasValue)
                query = query.Where(d => d.LastUsedAt >= lastUsedFrom.Value);

            if (lastUsedTo.HasValue)
                query = query.Where(d => d.LastUsedAt <= lastUsedTo.Value);

            return await query
                .OrderByDescending(d => d.LastUsedAt)
                .ThenByDescending(d => d.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
} 