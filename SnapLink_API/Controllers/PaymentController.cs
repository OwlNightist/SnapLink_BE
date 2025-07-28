using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Service.IService;
using Microsoft.Extensions.Configuration;

namespace SnapLink_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
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
            Console.WriteLine($"Error in CreatePaymentLink: {ex.Message}");
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
            Console.WriteLine($"Controller: Getting payment status for ID: {paymentId}");
            var result = await _paymentService.GetPaymentStatusAsync(paymentId);
            
            Console.WriteLine($"Controller: Service returned Error={result.Error}, Message={result.Message}");
            
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
            Console.WriteLine($"Error in GetPaymentStatus: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
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
            Console.WriteLine($"Error in CancelPayment: {ex.Message}");
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
        try
        {
            Console.WriteLine("🔍 WEBHOOK METHOD CALLED!");
            
            // Lấy checksumKey từ cấu hình
            var configuration = HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
            var checksumKey = configuration["PayOS:ChecksumKey"];
            if (string.IsNullOrEmpty(checksumKey))
            {
                Console.WriteLine("❌ Missing PayOS checksum key");
                return StatusCode(500, new { message = "Missing PayOS checksum key" });
            }

            // Log chi tiết để debug
            Console.WriteLine("=== WEBHOOK DEBUG START ===");
            Console.WriteLine($"Webhook received - orderCode: {payload.data?.orderCode}");
            Console.WriteLine($"ChecksumKey: {checksumKey}");
            Console.WriteLine($"Received signature: {payload.signature}");
            Console.WriteLine($"Full payload: {System.Text.Json.JsonSerializer.Serialize(payload)}");

            // Xác thực signature
            var signatureString = PayOSWebhookHelper.BuildSignatureString(payload.data);
            var computedSignature = PayOSWebhookHelper.ComputeHmacSha256(signatureString, checksumKey);
            
            Console.WriteLine($"Signature string: {signatureString}");
            Console.WriteLine($"Computed signature: {computedSignature}");
            Console.WriteLine($"Received signature: {payload.signature}");
            Console.WriteLine($"Signature match: {string.Equals(computedSignature, payload.signature, StringComparison.OrdinalIgnoreCase)}");
            Console.WriteLine("=== WEBHOOK DEBUG END ===");
            
            if (!string.Equals(computedSignature, payload.signature, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("❌ Signature validation failed!");
                return Unauthorized(new { message = "Invalid signature" });
            }

            Console.WriteLine("✅ Signature validation successful!");
            // Gọi service để xử lý cập nhật trạng thái payment/booking
            await paymentService.HandlePayOSWebhookAsync(payload);
            return Ok(new { message = "Webhook processed" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ WEBHOOK ERROR: {ex.Message}");
            Console.WriteLine($"❌ WEBHOOK STACK TRACE: {ex.StackTrace}");
            return StatusCode(500, new { message = $"Webhook error: {ex.Message}" });
        }
    }
} 