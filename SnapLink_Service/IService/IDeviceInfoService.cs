using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SnapLink_Service.IService
{
    public interface IDeviceInfoService
    {
        // Core operations
        Task<DeviceInfoResponse> CreateDeviceInfoAsync(CreateDeviceInfoRequest request);
        Task<DeviceInfoResponse> GetDeviceInfoByIdAsync(int deviceInfoId);
        Task<DeviceInfoResponse> UpdateDeviceInfoAsync(int deviceInfoId, UpdateDeviceInfoRequest request);
        Task<bool> DeleteDeviceInfoAsync(int deviceInfoId);

        // Photographer operations
        Task<IEnumerable<DeviceInfoResponse>> GetDeviceInfosByPhotographerIdAsync(int photographerId);
        Task<DeviceInfoResponse> AddDeviceToPhotographerAsync(int photographerId, CreateDeviceInfoRequest request);
        Task<bool> UpdateDeviceLastUsedAsync(int deviceInfoId);

        // Search and filter operations
        Task<IEnumerable<DeviceInfoResponse>> GetDeviceInfosByTypeAsync(string deviceType);
        Task<IEnumerable<DeviceInfoResponse>> GetDeviceInfosByBrandAsync(string brand);
        Task<IEnumerable<DeviceInfoResponse>> GetDeviceInfosByStatusAsync(string status);
        Task<IEnumerable<DeviceInfoResponse>> GetActiveDeviceInfosAsync();
        Task<IEnumerable<DeviceInfoResponse>> GetRecentlyUsedDevicesAsync(DateTime fromDate);

        // Advanced search
        Task<DeviceSearchResponse> SearchDeviceInfosAsync(DeviceSearchRequest request);

        // Analytics operations
        Task<DeviceAnalyticsResponse> GetDeviceAnalyticsAsync();
        Task<Dictionary<string, int>> GetDeviceTypeDistributionAsync();
        Task<Dictionary<string, int>> GetBrandDistributionAsync();
        Task<Dictionary<string, int>> GetCameraResolutionDistributionAsync();

        // Bulk operations
        Task<IEnumerable<DeviceInfoResponse>> CreateBulkDeviceInfosAsync(BulkDeviceInfoRequest request);

        // Business logic
        Task<bool> IsDeviceCompatibleWithBookingAsync(int deviceInfoId, int bookingId);
        Task<string> GetDeviceCapabilitiesAsync(int deviceInfoId);
        Task<bool> ValidateDeviceInfoAsync(CreateDeviceInfoRequest request);
        Task<string> GenerateDeviceReportAsync(int photographerId);
    }
} 