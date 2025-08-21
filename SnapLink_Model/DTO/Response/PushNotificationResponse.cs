namespace SnapLink_Model.DTO.Response;

public class DeviceResponse
{
    public int DeviceId { get; set; }
    public int UserId { get; set; }
    public string ExpoPushToken { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string? DeviceId_External { get; set; }
    public string? DeviceName { get; set; }
    public string? AppVersion { get; set; }
    public string? OsVersion { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
}

public class NotificationResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}

public class BulkNotificationResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int TotalSent { get; set; }
    public int TotalFailed { get; set; }
    public List<string> FailedTokens { get; set; } = new List<string>();
    public List<string> Errors { get; set; } = new List<string>();
}

public class ExpoNotificationPayload
{
    public string To { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public object? Data { get; set; }
    public string Sound { get; set; } = "default";
    public string Priority { get; set; } = "high";
}

public class ExpoApiResponse
{
    public class ExpoResponseData
    {
        public string Status { get; set; } = string.Empty;
        public string? Id { get; set; }
        public string? Message { get; set; }
        public ExpoErrorDetails? Details { get; set; }
    }

    public class ExpoErrorDetails
    {
        public string? Error { get; set; }
        public string? Message { get; set; }
    }

    public List<ExpoResponseData> Data { get; set; } = new List<ExpoResponseData>();
}
