using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SnapLink_Model.DTO.Request
{
    public class CreateDeviceInfoRequest
    {
        [Required]
        public int PhotographerId { get; set; }

        [Required]
        [MaxLength(100)]
        public string DeviceType { get; set; } = string.Empty; // "Phone", "Camera", "Tablet"

        [Required]
        [MaxLength(100)]
        public string Brand { get; set; } = string.Empty; // "Apple", "Samsung", "Canon", "Nikon"

        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty; // "iPhone 15 Pro", "Galaxy S24", "EOS R5"

        [MaxLength(20)]
        public string? OperatingSystem { get; set; } // "iOS 17", "Android 14"

        [MaxLength(20)]
        public string? OsVersion { get; set; } // "17.2.1", "14.0"

        [MaxLength(50)]
        public string? ScreenResolution { get; set; } // "2556x1179", "3088x1440"

        [MaxLength(20)]
        public string? CameraResolution { get; set; } // "48MP", "50MP"

        [MaxLength(100)]
        public string? StorageCapacity { get; set; } // "256GB", "1TB"

        [MaxLength(100)]
        public string? BatteryCapacity { get; set; } // "4441mAh", "5000mAh"

        [MaxLength(500)]
        public string? Features { get; set; } // JSON string: {"NightMode": true, "PortraitMode": true, "4KVideo": true}

        [MaxLength(500)]
        public string? Notes { get; set; } // Additional notes about the device
    }

    public class UpdateDeviceInfoRequest
    {
        [MaxLength(100)]
        public string? DeviceType { get; set; }

        [MaxLength(100)]
        public string? Brand { get; set; }

        [MaxLength(100)]
        public string? Model { get; set; }

        [MaxLength(20)]
        public string? OperatingSystem { get; set; }

        [MaxLength(20)]
        public string? OsVersion { get; set; }

        [MaxLength(50)]
        public string? ScreenResolution { get; set; }

        [MaxLength(20)]
        public string? CameraResolution { get; set; }

        [MaxLength(100)]
        public string? StorageCapacity { get; set; }

        [MaxLength(100)]
        public string? BatteryCapacity { get; set; }

        [MaxLength(500)]
        public string? Features { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    public class DeviceSearchRequest
    {
        public int? PhotographerId { get; set; }
        public string? DeviceType { get; set; }
        public string? Brand { get; set; }
        public string? Status { get; set; }
        public DateTime? LastUsedFrom { get; set; }
        public DateTime? LastUsedTo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class BulkDeviceInfoRequest
    {
        [Required]
        public int PhotographerId { get; set; }

        public List<CreateDeviceInfoRequest> Devices { get; set; } = new List<CreateDeviceInfoRequest>();
    }
} 