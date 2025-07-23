using Microsoft.AspNetCore.Mvc;
using SnapLink_Service.IService;

namespace SnapLink_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet("history/user/{userId}")]
    public async Task<IActionResult> GetTransactionHistoryByUser(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var transactions = await _transactionService.GetUserTransactionHistoryAsync(userId, page, pageSize);
            var totalCount = await _transactionService.GetUserTransactionCountAsync(userId);
            
            return Ok(new
            {
                Error = 0,
                Message = "Transaction history retrieved successfully",
                Data = new
                {
                    Transactions = transactions,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetTransactionHistoryByUser: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    [HttpGet("history/photographer/{photographerId}")]
    public async Task<IActionResult> GetTransactionHistoryByPhotographer(int photographerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var transactions = await _transactionService.GetPhotographerTransactionHistoryAsync(photographerId, page, pageSize);
            var totalCount = await _transactionService.GetPhotographerTransactionCountAsync(photographerId);
            
            return Ok(new
            {
                Error = 0,
                Message = "Transaction history retrieved successfully",
                Data = new
                {
                    Transactions = transactions,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetTransactionHistoryByPhotographer: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    [HttpGet("history/location-owner/{locationOwnerId}")]
    public async Task<IActionResult> GetTransactionHistoryByLocationOwner(int locationOwnerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var transactions = await _transactionService.GetLocationOwnerTransactionHistoryAsync(locationOwnerId, page, pageSize);
            var totalCount = await _transactionService.GetLocationOwnerTransactionCountAsync(locationOwnerId);
            
            return Ok(new
            {
                Error = 0,
                Message = "Transaction history retrieved successfully",
                Data = new
                {
                    Transactions = transactions,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetTransactionHistoryByLocationOwner: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    [HttpGet("{transactionId}")]
    public async Task<IActionResult> GetTransactionById(int transactionId)
    {
        try
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(transactionId);
            
            if (transaction == null)
            {
                return NotFound(new
                {
                    Error = -1,
                    Message = "Transaction not found",
                    Data = (object?)null
                });
            }
            
            return Ok(new
            {
                Error = 0,
                Message = "Transaction retrieved successfully",
                Data = transaction
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetTransactionById: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }
} 