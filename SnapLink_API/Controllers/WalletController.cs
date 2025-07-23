using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Service.IService;

namespace SnapLink_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetWalletBalance([FromQuery] int userId)
    {
        try
        {
            var balance = await _walletService.GetWalletBalanceAsync(userId);
            
            return Ok(new
            {
                Error = 0,
                Message = "Wallet balance retrieved successfully",
                Data = new { UserId = userId, Balance = balance }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetWalletBalance: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    [HttpGet("balance/{userId}")]
    public async Task<IActionResult> GetWalletBalanceByUserId(int userId)
    {
        try
        {
            var balance = await _walletService.GetWalletBalanceAsync(userId);
            
            return Ok(new
            {
                Error = 0,
                Message = "Wallet balance retrieved successfully",
                Data = new { UserId = userId, Balance = balance }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetWalletBalanceByUserId: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> TransferFunds([FromBody] TransferFundsRequest request)
    {
        try
        {
            var success = await _walletService.TransferFundsAsync(request.FromUserId, request.ToUserId, request.Amount);
            
            if (success)
            {
                return Ok(new
                {
                    Error = 0,
                    Message = "Funds transferred successfully",
                    Data = (object?)null
                });
            }
            else
            {
                return BadRequest(new
                {
                    Error = -1,
                    Message = "Transfer failed - insufficient funds or invalid user",
                    Data = (object?)null
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in TransferFunds: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }
}

public class TransferFundsRequest
{
    public int FromUserId { get; set; }
    public int ToUserId { get; set; }
    public decimal Amount { get; set; }
} 