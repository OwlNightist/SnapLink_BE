using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Service.IService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Net.payOS.Types;

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


    [HttpPost("webhook")]
    public async Task<IActionResult> PayOSWebhook([FromBody] WebhookType payload, [FromServices] IPaymentService paymentService)
    {
        // Log payload nhận được
        _logger.LogInformation($"Webhook received - Payload: {System.Text.Json.JsonSerializer.Serialize(payload)}");
        
        // Lấy PayOS instance từ DI
        var payOS = HttpContext.RequestServices.GetService(typeof(Net.payOS.PayOS)) as Net.payOS.PayOS;
        if (payOS == null)
        {
            _logger.LogError("PayOS service not found in DI");
            return StatusCode(500, new { message = "PayOS service not configured" });
        }
        
        try
        {
            // Sử dụng SDK để verify webhook
            WebhookData verifiedData = payOS.verifyPaymentWebhookData(payload);
            _logger.LogInformation("Webhook verification successful using SDK");
            
            // Gọi service để xử lý cập nhật trạng thái payment/booking
            await paymentService.HandlePayOSWebhookAsync(payload);
            return Ok(new { message = "Webhook processed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Webhook verification failed using SDK");
            return Unauthorized(new { message = "Invalid webhook signature" });
        }
    }
} 