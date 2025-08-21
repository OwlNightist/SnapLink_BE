using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Service.IService;

namespace SnapLink_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PushNotificationController : ControllerBase
{
    private readonly IPushNotificationService _pushNotificationService;
    private readonly ILogger<PushNotificationController> _logger;

    public PushNotificationController(
        IPushNotificationService pushNotificationService,
        ILogger<PushNotificationController> logger)
    {
        _pushNotificationService = pushNotificationService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new device for push notifications
    /// </summary>
    [HttpPost("register-device")]
    public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { 
                    message = "Invalid request data", 
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) 
                });
            }

            var result = await _pushNotificationService.RegisterDeviceAsync(request);
            
            return Ok(new { 
                message = "Device registered successfully", 
                data = result 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering device for user {UserId}", request.UserId);
            return StatusCode(500, new { 
                message = "Internal server error occurred while registering device" 
            });
        }
    }

    /// <summary>
    /// Update device information
    /// </summary>
    [HttpPut("device/{deviceId}")]
    public async Task<IActionResult> UpdateDevice(int deviceId, [FromBody] UpdateDeviceRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { 
                    message = "Invalid request data", 
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) 
                });
            }

            var result = await _pushNotificationService.UpdateDeviceAsync(deviceId, request);
            
            return Ok(new { 
                message = "Device updated successfully", 
                data = result 
            });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating device {DeviceId}", deviceId);
            return StatusCode(500, new { 
                message = "Internal server error occurred while updating device" 
            });
        }
    }

    /// <summary>
    /// Remove a device by ID
    /// </summary>
    [HttpDelete("device/{deviceId}")]
    public async Task<IActionResult> RemoveDevice(int deviceId)
    {
        try
        {
            var result = await _pushNotificationService.RemoveDeviceAsync(deviceId);
            
            if (!result)
            {
                return NotFound(new { message = "Device not found" });
            }

            return Ok(new { message = "Device removed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing device {DeviceId}", deviceId);
            return StatusCode(500, new { 
                message = "Internal server error occurred while removing device" 
            });
        }
    }

    /// <summary>
    /// Remove a device by Expo push token
    /// </summary>
    [HttpDelete("device/token/{expoPushToken}")]
    public async Task<IActionResult> RemoveDeviceByToken(string expoPushToken)
    {
        try
        {
            var result = await _pushNotificationService.RemoveDeviceByTokenAsync(expoPushToken);
            
            if (!result)
            {
                return NotFound(new { message = "Device not found" });
            }

            return Ok(new { message = "Device removed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing device by token");
            return StatusCode(500, new { 
                message = "Internal server error occurred while removing device" 
            });
        }
    }

    /// <summary>
    /// Get all devices for a user
    /// </summary>
    [HttpGet("user/{userId}/devices")]
    public async Task<IActionResult> GetUserDevices(int userId)
    {
        try
        {
            var devices = await _pushNotificationService.GetUserDevicesAsync(userId);
            
            return Ok(new { 
                message = "Devices retrieved successfully", 
                data = devices 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting devices for user {UserId}", userId);
            return StatusCode(500, new { 
                message = "Internal server error occurred while retrieving devices" 
            });
        }
    }

    /// <summary>
    /// Get device by Expo push token
    /// </summary>
    [HttpGet("device/token/{expoPushToken}")]
    public async Task<IActionResult> GetDeviceByToken(string expoPushToken)
    {
        try
        {
            var device = await _pushNotificationService.GetDeviceByTokenAsync(expoPushToken);
            
            if (device == null)
            {
                return NotFound(new { message = "Device not found" });
            }

            return Ok(new { 
                message = "Device retrieved successfully", 
                data = device 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting device by token");
            return StatusCode(500, new { 
                message = "Internal server error occurred while retrieving device" 
            });
        }
    }

    /// <summary>
    /// Send a notification to a specific user
    /// </summary>
    [HttpPost("send")]
    [Authorize] // Add authorization as needed
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { 
                    message = "Invalid request data", 
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) 
                });
            }

            var result = await _pushNotificationService.SendNotificationToUserAsync(
                request.UserId, 
                request.Title, 
                request.Body, 
                request.Data);
            
            if (!result.Success)
            {
                return BadRequest(new { 
                    message = result.Message, 
                    errors = result.Errors 
                });
            }

            return Ok(new { 
                message = result.Message, 
                data = result.Data 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to user {UserId}", request.UserId);
            return StatusCode(500, new { 
                message = "Internal server error occurred while sending notification" 
            });
        }
    }

    /// <summary>
    /// Send bulk notifications to multiple users
    /// </summary>
    [HttpPost("send-bulk")]
    [Authorize] // Add authorization as needed
    public async Task<IActionResult> SendBulkNotification([FromBody] SendBulkNotificationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { 
                    message = "Invalid request data", 
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) 
                });
            }

            var result = await _pushNotificationService.SendBulkNotificationToUsersAsync(
                request.UserIds, 
                request.Title, 
                request.Body, 
                request.Data);
            
            return Ok(new { 
                message = result.Message, 
                totalSent = result.TotalSent,
                totalFailed = result.TotalFailed,
                errors = result.Errors
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending bulk notification");
            return StatusCode(500, new { 
                message = "Internal server error occurred while sending bulk notification" 
            });
        }
    }

    /// <summary>
    /// Update last used timestamp for a device
    /// </summary>
    [HttpPut("device/token/{expoPushToken}/last-used")]
    public async Task<IActionResult> UpdateLastUsed(string expoPushToken)
    {
        try
        {
            var result = await _pushNotificationService.UpdateLastUsedAsync(expoPushToken);
            
            if (!result)
            {
                return NotFound(new { message = "Device not found" });
            }

            return Ok(new { message = "Last used timestamp updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating last used for device");
            return StatusCode(500, new { 
                message = "Internal server error occurred while updating device" 
            });
        }
    }

    /// <summary>
    /// Validate an Expo push token
    /// </summary>
    [HttpPost("validate-token")]
    public async Task<IActionResult> ValidateToken([FromBody] string expoPushToken)
    {
        try
        {
            var isValid = await _pushNotificationService.ValidateExpoPushTokenAsync(expoPushToken);
            
            return Ok(new { 
                isValid = isValid,
                message = isValid ? "Token is valid" : "Token is invalid"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return StatusCode(500, new { 
                message = "Internal server error occurred while validating token" 
            });
        }
    }

    /// <summary>
    /// Cleanup inactive/invalid tokens
    /// </summary>
    [HttpPost("cleanup-tokens")]
    [Authorize] // Add proper admin authorization
    public async Task<IActionResult> CleanupTokens()
    {
        try
        {
            var deletedCount = await _pushNotificationService.CleanupInvalidTokensAsync();
            
            return Ok(new { 
                message = "Token cleanup completed successfully",
                deletedCount = deletedCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token cleanup");
            return StatusCode(500, new { 
                message = "Internal server error occurred during token cleanup" 
            });
        }
    }
}
