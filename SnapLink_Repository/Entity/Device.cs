using System;
using System.ComponentModel.DataAnnotations;

namespace SnapLink_Repository.Entity;

/// <summary>
/// Device entity for managing push notification tokens and device information
/// </summary>
public partial class Device
{
    public int DeviceId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(255)]
    public string ExpoPushToken { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string DeviceType { get; set; } = string.Empty; // "android", "ios"

    [MaxLength(255)]
    public string? DeviceId_External { get; set; } // External device identifier

    [MaxLength(100)]
    public string? DeviceName { get; set; } // "iPhone 15 Pro", "Samsung Galaxy S24"

    [MaxLength(50)]
    public string? AppVersion { get; set; } // App version

    [MaxLength(50)]
    public string? OsVersion { get; set; } // OS version

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastUsedAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
}
