using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Request;

public class RegisterDeviceRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(255)]
    public string ExpoPushToken { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string DeviceType { get; set; } = string.Empty; // "android", "ios"

    [MaxLength(255)]
    public string? DeviceId { get; set; }

    [MaxLength(100)]
    public string? DeviceName { get; set; }

    [MaxLength(50)]
    public string? AppVersion { get; set; }

    [MaxLength(50)]
    public string? OsVersion { get; set; }
}

public class UpdateDeviceRequest
{
    [MaxLength(255)]
    public string? ExpoPushToken { get; set; }

    [MaxLength(100)]
    public string? DeviceName { get; set; }

    [MaxLength(50)]
    public string? AppVersion { get; set; }

    [MaxLength(50)]
    public string? OsVersion { get; set; }

    public bool? IsActive { get; set; }
}

public class SendNotificationRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Body { get; set; } = string.Empty;

    public object? Data { get; set; }

    [MaxLength(50)]
    public string? Sound { get; set; } = "default";

    [MaxLength(20)]
    public string? Priority { get; set; } = "high";
}

public class SendBulkNotificationRequest
{
    [Required]
    public List<int> UserIds { get; set; } = new List<int>();

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Body { get; set; } = string.Empty;

    public object? Data { get; set; }

    [MaxLength(50)]
    public string? Sound { get; set; } = "default";

    [MaxLength(20)]
    public string? Priority { get; set; } = "high";
}
