using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService;

public interface IPushNotificationService
{
    // Device Management
    Task<DeviceResponse> RegisterDeviceAsync(RegisterDeviceRequest request);
    Task<DeviceResponse> UpdateDeviceAsync(int deviceId, UpdateDeviceRequest request);
    Task<bool> RemoveDeviceAsync(int deviceId);
    Task<bool> RemoveDeviceByTokenAsync(string expoPushToken);
    Task<IEnumerable<DeviceResponse>> GetUserDevicesAsync(int userId);
    Task<DeviceResponse?> GetDeviceByTokenAsync(string expoPushToken);
    Task<bool> UpdateLastUsedAsync(string expoPushToken);

    // Notification Sending
    Task<NotificationResponse> SendNotificationAsync(string expoPushToken, string title, string body, object? data = null);
    Task<NotificationResponse> SendNotificationToUserAsync(int userId, string title, string body, object? data = null);
    Task<BulkNotificationResponse> SendBulkNotificationAsync(List<string> expoPushTokens, string title, string body, object? data = null);
    Task<BulkNotificationResponse> SendBulkNotificationToUsersAsync(List<int> userIds, string title, string body, object? data = null);

    // Business Logic Notifications
    Task<NotificationResponse> SendBookingNotificationAsync(int photographerId, string customerName, int bookingId);
    Task<NotificationResponse> SendMessageNotificationAsync(int recipientId, string senderName, string messageContent, int conversationId);
    Task<NotificationResponse> SendPaymentNotificationAsync(int userId, string status, decimal amount, int paymentId);
    Task<NotificationResponse> SendBookingStatusNotificationAsync(int userId, string status, int bookingId);
    Task<NotificationResponse> SendPhotographerApplicationNotificationAsync(int locationOwnerId, string photographerName, int eventId);
    Task<NotificationResponse> SendApplicationResponseNotificationAsync(int photographerId, string eventName, string status, int eventId);

    // Utility Methods
    Task<bool> ValidateExpoPushTokenAsync(string expoPushToken);
    Task<int> CleanupInvalidTokensAsync();
    Task<List<string>> GetActiveTokensForUserAsync(int userId);
}
