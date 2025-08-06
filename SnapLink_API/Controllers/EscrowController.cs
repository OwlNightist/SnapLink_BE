using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Service.IService;
using System.Security.Claims;

namespace SnapLink_API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EscrowController : ControllerBase
{
    private readonly IEscrowService _escrowService;

    public EscrowController(IEscrowService escrowService)
    {
        _escrowService = escrowService;
    }

    [HttpGet("balance/{bookingId}")]
    public async Task<IActionResult> GetEscrowBalance(int bookingId)
    {
        try
        {
            var balance = await _escrowService.GetEscrowBalanceAsync(bookingId);
            
            return Ok(new
            {
                Error = 0,
                Message = "Escrow balance retrieved successfully",
                Data = new
                {
                    BookingId = bookingId,
                    EscrowBalance = balance
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetEscrowBalance: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    [HttpGet("transactions/{bookingId}")]
    public async Task<IActionResult> GetEscrowTransactions(int bookingId)
    {
        try
        {
            var transactions = await _escrowService.GetEscrowTransactionsAsync(bookingId);
            
            return Ok(new
            {
                Error = 0,
                Message = "Escrow transactions retrieved successfully",
                Data = new
                {
                    BookingId = bookingId,
                    Transactions = transactions
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetEscrowTransactions: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }
} 