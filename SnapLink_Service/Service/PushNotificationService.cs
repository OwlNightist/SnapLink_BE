using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Repository.DBContext;
using SnapLink_Repository.Entity;
using SnapLink_Repository.Repository;
using SnapLink_Service.IService;

namespace SnapLink_Service.Service;

public class PushNotificationService : IPushNotificationService
{
    private readonly SnaplinkDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly ILogger<PushNotificationService> _logger;
    private readonly string _expoPushUrl = "https://exp.host/--/api/v2/push/send";

    public PushNotificationService(
        SnaplinkDbContext context,
        HttpClient httpClient,
        ILogger<PushNotificationService> logger)
    {
        _context = context;
        _httpClient = httpClient;
        _logger = logger;
    }

    #region Device Management

    public async Task<DeviceResponse> RegisterDeviceAsync(RegisterDeviceRequest request)
    {
        try
        {
            // Check if device already exists
            var existingDevice = await _context.Devices
                .FirstOrDefaultAsync(d => d.UserId == request.UserId && 
                                        d.ExpoPushToken == request.ExpoPushToken);

            if (existingDevice != null)
            {
                // Update existing device
                existingDevice.DeviceType = request.DeviceType;
                existingDevice.DeviceId_External = request.DeviceId;
                existingDevice.DeviceName = request.DeviceName;
                existingDevice.AppVersion = request.AppVersion;
                existingDevice.OsVersion = request.OsVersion;
                existingDevice.IsActive = true;
                existingDevice.UpdatedAt = DateTime.UtcNow;
                existingDevice.LastUsedAt = DateTime.UtcNow;

                _context.Devices.Update(existingDevice);
                await _context.SaveChangesAsync();

                return MapToDeviceResponse(existingDevice);
            }

            // Create new device
            var device = new Device
            {
                UserId = request.UserId,
                ExpoPushToken = request.ExpoPushToken,
                DeviceType = request.DeviceType,
                DeviceId_External = request.DeviceId,
                DeviceName = request.DeviceName,
                AppVersion = request.AppVersion,
                OsVersion = request.OsVersion,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                LastUsedAt = DateTime.UtcNow
            };

            await _context.Devices.AddAsync(device);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Device registered successfully for User {UserId} with token {Token}", 
                request.UserId, request.ExpoPushToken[..10] + "...");

            return MapToDeviceResponse(device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering device for user {UserId}", request.UserId);
            throw;
        }
    }

    public async Task<DeviceResponse> UpdateDeviceAsync(int deviceId, UpdateDeviceRequest request)
    {
        try
        {
            var device = await _context.Devices.FindAsync(deviceId);
            if (device == null)
                throw new ArgumentException($"Device with ID {deviceId} not found");

            if (!string.IsNullOrEmpty(request.ExpoPushToken))
                device.ExpoPushToken = request.ExpoPushToken;
            
            if (!string.IsNullOrEmpty(request.DeviceName))
                device.DeviceName = request.DeviceName;
            
            if (!string.IsNullOrEmpty(request.AppVersion))
                device.AppVersion = request.AppVersion;
            
            if (!string.IsNullOrEmpty(request.OsVersion))
                device.OsVersion = request.OsVersion;
            
            if (request.IsActive.HasValue)
                device.IsActive = request.IsActive.Value;

            device.UpdatedAt = DateTime.UtcNow;

            _context.Devices.Update(device);
            await _context.SaveChangesAsync();

            return MapToDeviceResponse(device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating device {DeviceId}", deviceId);
            throw;
        }
    }

    public async Task<bool> RemoveDeviceAsync(int deviceId)
    {
        try
        {
            var device = await _context.Devices.FindAsync(deviceId);
            if (device == null)
                return false;

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Device {DeviceId} removed successfully", deviceId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing device {DeviceId}", deviceId);
            return false;
        }
    }

    public async Task<bool> RemoveDeviceByTokenAsync(string expoPushToken)
    {
        try
        {
            var device = await _context.Devices
                .FirstOrDefaultAsync(d => d.ExpoPushToken == expoPushToken);
            
            if (device == null)
                return false;

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Device with token {Token} removed successfully", 
                expoPushToken[..10] + "...");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing device by token");
            return false;
        }
    }

    public async Task<IEnumerable<DeviceResponse>> GetUserDevicesAsync(int userId)
    {
        try
        {
            var devices = await _context.Devices
                .Where(d => d.UserId == userId && d.IsActive)
                .OrderByDescending(d => d.LastUsedAt)
                .ToListAsync();

            return devices.Select(MapToDeviceResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting devices for user {UserId}", userId);
            throw;
        }
    }

    public async Task<DeviceResponse?> GetDeviceByTokenAsync(string expoPushToken)
    {
        try
        {
            var device = await _context.Devices
                .FirstOrDefaultAsync(d => d.ExpoPushToken == expoPushToken && d.IsActive);

            return device == null ? null : MapToDeviceResponse(device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting device by token");
            throw;
        }
    }

    public async Task<bool> UpdateLastUsedAsync(string expoPushToken)
    {
        try
        {
            var device = await _context.Devices
                .FirstOrDefaultAsync(d => d.ExpoPushToken == expoPushToken && d.IsActive);

            if (device == null)
                return false;

            device.LastUsedAt = DateTime.UtcNow;
            device.UpdatedAt = DateTime.UtcNow;

            _context.Devices.Update(device);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating last used for device");
            return false;
        }
    }

    #endregion

    #region Notification Sending

    public async Task<NotificationResponse> SendNotificationAsync(string expoPushToken, string title, string body, object? data = null)
    {
        try
        {
            if (!await ValidateExpoPushTokenAsync(expoPushToken))
            {
                return new NotificationResponse
                {
                    Success = false,
                    Message = "Invalid Expo Push Token",
                    Errors = { "Token format is invalid" }
                };
            }

            var payload = new ExpoNotificationPayload
            {
                To = expoPushToken,
                Title = title,
                Body = body,
                Data = data,
                Sound = "default",
                Priority = "high"
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_expoPushUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("✅ Notification sent successfully to {Token}: {Title}", 
                    expoPushToken[..10] + "...", title);
                
                // Update last used
                await UpdateLastUsedAsync(expoPushToken);

                return new NotificationResponse
                {
                    Success = true,
                    Message = "Notification sent successfully",
                    Data = responseContent
                };
            }
            else
            {
                _logger.LogWarning("❌ Failed to send notification to {Token}: {StatusCode} - {Response}", 
                    expoPushToken[..10] + "...", response.StatusCode, responseContent);

                return new NotificationResponse
                {
                    Success = false,
                    Message = $"Failed to send notification: {response.StatusCode}",
                    Errors = { responseContent }
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error sending notification to {Token}", expoPushToken[..10] + "...");
            return new NotificationResponse
            {
                Success = false,
                Message = "Error sending notification",
                Errors = { ex.Message }
            };
        }
    }

    public async Task<NotificationResponse> SendNotificationToUserAsync(int userId, string title, string body, object? data = null)
    {
        try
        {
            var activeTokens = await GetActiveTokensForUserAsync(userId);
            
            if (!activeTokens.Any())
            {
                return new NotificationResponse
                {
                    Success = false,
                    Message = "No active devices found for user",
                    Errors = { "User has no registered devices" }
                };
            }

            var results = new List<NotificationResponse>();
            
            foreach (var token in activeTokens)
            {
                var result = await SendNotificationAsync(token, title, body, data);
                results.Add(result);
            }

            var successCount = results.Count(r => r.Success);
            var allSuccessful = successCount == results.Count;

            return new NotificationResponse
            {
                Success = allSuccessful,
                Message = $"Sent to {successCount}/{results.Count} devices",
                Data = results
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
            return new NotificationResponse
            {
                Success = false,
                Message = "Error sending notification to user",
                Errors = { ex.Message }
            };
        }
    }

    public async Task<BulkNotificationResponse> SendBulkNotificationAsync(List<string> expoPushTokens, string title, string body, object? data = null)
    {
        try
        {
            var notifications = expoPushTokens.Select(token => new ExpoNotificationPayload
            {
                To = token,
                Title = title,
                Body = body,
                Data = data,
                Sound = "default",
                Priority = "high"
            }).ToArray();

            var json = JsonSerializer.Serialize(notifications);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_expoPushUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("✅ Bulk notification sent to {Count} devices: {Title}", 
                    expoPushTokens.Count, title);

                // Update last used for all tokens
                foreach (var token in expoPushTokens)
                {
                    await UpdateLastUsedAsync(token);
                }

                return new BulkNotificationResponse
                {
                    Success = true,
                    Message = "Bulk notification sent successfully",
                    TotalSent = expoPushTokens.Count,
                    TotalFailed = 0
                };
            }
            else
            {
                _logger.LogWarning("❌ Failed to send bulk notification: {StatusCode} - {Response}", 
                    response.StatusCode, responseContent);

                return new BulkNotificationResponse
                {
                    Success = false,
                    Message = $"Failed to send bulk notification: {response.StatusCode}",
                    TotalSent = 0,
                    TotalFailed = expoPushTokens.Count,
                    Errors = { responseContent }
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error sending bulk notification");
            return new BulkNotificationResponse
            {
                Success = false,
                Message = "Error sending bulk notification",
                TotalSent = 0,
                TotalFailed = expoPushTokens.Count,
                Errors = { ex.Message }
            };
        }
    }

    public async Task<BulkNotificationResponse> SendBulkNotificationToUsersAsync(List<int> userIds, string title, string body, object? data = null)
    {
        try
        {
            var allTokens = new List<string>();
            
            foreach (var userId in userIds)
            {
                var userTokens = await GetActiveTokensForUserAsync(userId);
                allTokens.AddRange(userTokens);
            }

            if (!allTokens.Any())
            {
                return new BulkNotificationResponse
                {
                    Success = false,
                    Message = "No active devices found for any users",
                    TotalSent = 0,
                    TotalFailed = userIds.Count
                };
            }

            return await SendBulkNotificationAsync(allTokens, title, body, data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending bulk notification to users");
            return new BulkNotificationResponse
            {
                Success = false,
                Message = "Error sending bulk notification to users",
                TotalSent = 0,
                TotalFailed = userIds.Count,
                Errors = { ex.Message }
            };
        }
    }

    #endregion

    #region Business Logic Notifications

    public async Task<NotificationResponse> SendBookingNotificationAsync(int photographerId, string customerName, int bookingId)
    {
        return await SendNotificationToUserAsync(
            photographerId,
            "Booking mới!",
            $"Bạn có booking mới từ {customerName}",
            new
            {
                screen = "BookingDetail",
                bookingId = bookingId.ToString(),
                type = "NEW_BOOKING"
            }
        );
    }

    public async Task<NotificationResponse> SendMessageNotificationAsync(int recipientId, string senderName, string messageContent, int conversationId)
    {
        return await SendNotificationToUserAsync(
            recipientId,
            "Tin nhắn mới",
            $"{senderName}: {messageContent}",
            new
            {
                screen = "ChatScreen",
                conversationId = conversationId.ToString(),
                type = "NEW_MESSAGE"
            }
        );
    }

    public async Task<NotificationResponse> SendPaymentNotificationAsync(int userId, string status, decimal amount, int paymentId)
    {
        var title = status.ToLower() switch
        {
            "success" => "Thanh toán thành công",
            "failed" => "Thanh toán thất bại",
            "cancelled" => "Thanh toán đã hủy",
            _ => "Cập nhật thanh toán"
        };

        var body = status.ToLower() switch
        {
            "success" => $"Bạn đã thanh toán thành công {amount:N0} VND",
            "failed" => $"Thanh toán {amount:N0} VND không thành công",
            "cancelled" => $"Thanh toán {amount:N0} VND đã bị hủy",
            _ => $"Trạng thái thanh toán {amount:N0} VND đã được cập nhật"
        };

        return await SendNotificationToUserAsync(
            userId,
            title,
            body,
            new
            {
                screen = "PaymentDetail",
                paymentId = paymentId.ToString(),
                type = "PAYMENT_UPDATE",
                status = status
            }
        );
    }

    public async Task<NotificationResponse> SendBookingStatusNotificationAsync(int userId, string status, int bookingId)
    {
        var title = status.ToLower() switch
        {
            "confirmed" => "Booking đã được xác nhận",
            "completed" => "Booking đã hoàn thành",
            "cancelled" => "Booking đã bị hủy",
            _ => "Cập nhật booking"
        };

        var body = status.ToLower() switch
        {
            "confirmed" => "Photographer đã xác nhận booking của bạn",
            "completed" => "Booking của bạn đã hoàn thành thành công",
            "cancelled" => "Booking của bạn đã bị hủy",
            _ => $"Trạng thái booking của bạn đã được cập nhật thành {status}"
        };

        return await SendNotificationToUserAsync(
            userId,
            title,
            body,
            new
            {
                screen = "BookingDetail",
                bookingId = bookingId.ToString(),
                type = "BOOKING_STATUS_UPDATE",
                status = status
            }
        );
    }

    public async Task<NotificationResponse> SendPhotographerApplicationNotificationAsync(int locationOwnerId, string photographerName, int eventId)
    {
        return await SendNotificationToUserAsync(
            locationOwnerId,
            "Đơn đăng ký mới",
            $"{photographerName} đã đăng ký tham gia sự kiện của bạn",
            new
            {
                screen = "EventApplications",
                eventId = eventId.ToString(),
                type = "NEW_APPLICATION"
            }
        );
    }

    public async Task<NotificationResponse> SendApplicationResponseNotificationAsync(int photographerId, string eventName, string status, int eventId)
    {
        var title = status.ToLower() switch
        {
            "approved" => "Đơn đăng ký được chấp nhận",
            "rejected" => "Đơn đăng ký bị từ chối",
            _ => "Cập nhật đơn đăng ký"
        };

        var body = status.ToLower() switch
        {
            "approved" => $"Đơn đăng ký của bạn cho sự kiện '{eventName}' đã được chấp nhận",
            "rejected" => $"Đơn đăng ký của bạn cho sự kiện '{eventName}' đã bị từ chối",
            _ => $"Trạng thái đơn đăng ký cho sự kiện '{eventName}' đã được cập nhật"
        };

        return await SendNotificationToUserAsync(
            photographerId,
            title,
            body,
            new
            {
                screen = "EventDetail",
                eventId = eventId.ToString(),
                type = "APPLICATION_RESPONSE",
                status = status
            }
        );
    }

    #endregion

    #region Utility Methods

    public async Task<bool> ValidateExpoPushTokenAsync(string expoPushToken)
    {
        // Basic validation for Expo Push Token format
        if (string.IsNullOrWhiteSpace(expoPushToken))
            return false;

        // Expo Push Tokens typically start with "ExponentPushToken[" or "ExpoPushToken["
        return expoPushToken.StartsWith("ExponentPushToken[") || 
               expoPushToken.StartsWith("ExpoPushToken[") ||
               expoPushToken.StartsWith("expo-push-token://");
    }

    public async Task<int> CleanupInvalidTokensAsync()
    {
        try
        {
            var inactiveDevices = await _context.Devices
                .Where(d => !d.IsActive || d.LastUsedAt < DateTime.UtcNow.AddDays(-30))
                .ToListAsync();

            _context.Devices.RemoveRange(inactiveDevices);
            var deletedCount = await _context.SaveChangesAsync();

            _logger.LogInformation("Cleaned up {Count} inactive devices", deletedCount);
            return deletedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up invalid tokens");
            return 0;
        }
    }

    public async Task<List<string>> GetActiveTokensForUserAsync(int userId)
    {
        try
        {
            return await _context.Devices
                .Where(d => d.UserId == userId && d.IsActive)
                .Select(d => d.ExpoPushToken)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active tokens for user {UserId}", userId);
            return new List<string>();
        }
    }

    #endregion

    #region Helper Methods

    private static DeviceResponse MapToDeviceResponse(Device device)
    {
        return new DeviceResponse
        {
            DeviceId = device.DeviceId,
            UserId = device.UserId,
            ExpoPushToken = device.ExpoPushToken,
            DeviceType = device.DeviceType,
            DeviceId_External = device.DeviceId_External,
            DeviceName = device.DeviceName,
            AppVersion = device.AppVersion,
            OsVersion = device.OsVersion,
            IsActive = device.IsActive,
            CreatedAt = device.CreatedAt,
            UpdatedAt = device.UpdatedAt,
            LastUsedAt = device.LastUsedAt
        };
    }

    #endregion
}
