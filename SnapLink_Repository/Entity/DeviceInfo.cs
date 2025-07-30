using System;
using System.ComponentModel.DataAnnotations;

namespace SnapLink_Repository.Entity;

public partial class DeviceInfo
{
    public int DeviceInfoId { get; set; }

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

    [MaxLength(20)]
    public string Status { get; set; } = "Active"; // "Active", "Inactive", "Maintenance"

    [MaxLength(500)]
    public string? Notes { get; set; } // Additional notes about the device

    public DateTime? LastUsedAt { get; set; } // When the device was last used for booking

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Photographer Photographer { get; set; } = null!;
} 