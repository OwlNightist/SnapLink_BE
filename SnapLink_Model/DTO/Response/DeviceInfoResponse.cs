using System;
using System.Collections.Generic;

namespace SnapLink_Model.DTO.Response
{
    public class DeviceInfoResponse
    {
        public int DeviceInfoId { get; set; }
        public int PhotographerId { get; set; }
        public string DeviceType { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string? OperatingSystem { get; set; }
        public string? OsVersion { get; set; }
        public string? ScreenResolution { get; set; }
        public string? CameraResolution { get; set; }
        public string? StorageCapacity { get; set; }
        public string? BatteryCapacity { get; set; }
        public string? Features { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime? LastUsedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Photographer information
        public string? PhotographerName { get; set; }
        public string? PhotographerEmail { get; set; }
    }

    public class DeviceInfoDetailResponse : DeviceInfoResponse
    {
        public string? PhotographerPhone { get; set; }
        public int? PhotographerExperience { get; set; }
        public string? PhotographerEquipment { get; set; }
        public decimal? PhotographerRating { get; set; }
    }

    public class DeviceInfoSummaryResponse
    {
        public int DeviceInfoId { get; set; }
        public string DeviceType { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string? CameraResolution { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? LastUsedAt { get; set; }
        public string? PhotographerName { get; set; }
    }

    public class DeviceAnalyticsResponse
    {
        public int TotalDevices { get; set; }
        public int ActiveDevices { get; set; }
        public int InactiveDevices { get; set; }
        public int MaintenanceDevices { get; set; }
        
        // Device type distribution
        public int PhoneCount { get; set; }
        public int CameraCount { get; set; }
        public int TabletCount { get; set; }
        
        // Brand distribution
        public Dictionary<string, int> BrandDistribution { get; set; } = new Dictionary<string, int>();
        
        // Camera resolution distribution
        public Dictionary<string, int> CameraResolutionDistribution { get; set; } = new Dictionary<string, int>();
        
        // Recent activity
        public int DevicesUsedLastWeek { get; set; }
        public int DevicesUsedLastMonth { get; set; }
    }

    public class DeviceSearchResponse
    {
        public List<DeviceInfoResponse> Devices { get; set; } = new List<DeviceInfoResponse>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
} 