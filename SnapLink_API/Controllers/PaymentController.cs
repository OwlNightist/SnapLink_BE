using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Service.IService;

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
} 