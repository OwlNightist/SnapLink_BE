using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Service.IService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SnapLink_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }
// need userid for now , add to jwt token later
    [HttpPost("create")]
    public async Task<IActionResult> CreatePaymentLink([FromBody] CreatePaymentLinkRequest request, [FromQuery] int userId)
    {
        try
        {
            var result = await _paymentService.CreatePaymentLinkAsync(request, userId);
            
            if (result.Error == 0)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreatePaymentLink");
            return StatusCode(500, new PaymentResponse
            {
                Error = -1,
                Message = "Internal server error",
                Data = null
            });
        }
    }

    [HttpGet("{paymentId}")]
    public async Task<IActionResult> GetPaymentStatus([FromRoute] int paymentId)
    {
        try
        {
            _logger.LogInformation($"Controller: Getting payment status for ID: {paymentId}");
            var result = await _paymentService.GetPaymentStatusAsync(paymentId);
            
            _logger.LogInformation($"Controller: Service returned Error={result.Error}, Message={result.Message}");
            
            if (result.Error == 0)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in GetPaymentStatus for paymentId: {paymentId}");
            _logger.LogDebug($"Stack trace: {ex.StackTrace}");
            return StatusCode(500, new PaymentResponse
            {
                Error = -1,
                Message = "Internal server error",
                Data = null
            });
        }
    }

    [HttpPut("{paymentId}/cancel")]
    public async Task<IActionResult> CancelPayment([FromRoute] int paymentId)
    {
        try
        {
            var result = await _paymentService.CancelPaymentAsync(paymentId);
            
            if (result.Error == 0)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in CancelPayment for paymentId: {paymentId}");
            return StatusCode(500, new PaymentResponse
            {
                Error = -1,
                Message = "Internal server error",
                Data = null
            });
        }
    }

    [HttpGet("success")]
    public IActionResult PaymentSuccess([FromQuery] string? code, [FromQuery] string? id, [FromQuery] string? orderCode, [FromQuery] string? status)
    {
        // Chỉ trả về thông báo, không cập nhật trạng thái
        return Ok(new {
            message = "Cảm ơn bạn đã thanh toán. Hệ thống sẽ xác nhận giao dịch khi nhận được thông báo từ PayOS.",
            orderCode = orderCode,
            status = "pending"
        });
    }

    [HttpGet("cancel")]
    public IActionResult PaymentCancel([FromQuery] string? code, [FromQuery] string? id, [FromQuery] string? orderCode, [FromQuery] string? status)
    {
        // Chỉ trả về thông báo, không cập nhật trạng thái
        return Ok(new {
            message = "Bạn đã hủy thanh toán hoặc giao dịch chưa hoàn tất.",
                    orderCode = orderCode,
            status = "cancelled"
        });
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> PayOSWebhook([FromBody] PayOSWebhookRequest payload, [FromServices] IPaymentService paymentService)
    {
        // Log payload nhận được
        _logger.LogInformation($"Webhook received - Payload: {System.Text.Json.JsonSerializer.Serialize(payload)}");
        
        // Lấy checksumKey từ cấu hình
        var configuration = HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
        var checksumKey = configuration["PayOS:ChecksumKey"];
        if (string.IsNullOrEmpty(checksumKey))
        {
            _logger.LogError("Missing PayOS checksum key");
            return StatusCode(500, new { message = "Missing PayOS checksum key" });
        }
        
        _logger.LogInformation($"ChecksumKey: {checksumKey}");

        // Xác thực signature
        var signatureString = PayOSWebhookHelper.BuildSignatureString(payload.data);
        var computedSignature = PayOSWebhookHelper.ComputeHmacSha256(signatureString, checksumKey);
        
        _logger.LogInformation($"Signature string: {signatureString}");
        _logger.LogInformation($"Computed signature: {computedSignature}");
        _logger.LogInformation($"Received signature: {payload.signature}");
        
        if (!string.Equals(computedSignature, payload.signature, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogError("Signature verification failed");
            return Unauthorized(new { message = "Invalid signature" });
        }

        _logger.LogInformation("Signature verification successful");
        // Gọi service để xử lý cập nhật trạng thái payment/booking
        await paymentService.HandlePayOSWebhookAsync(payload);
        return Ok(new { message = "Webhook processed" });
    }
} 