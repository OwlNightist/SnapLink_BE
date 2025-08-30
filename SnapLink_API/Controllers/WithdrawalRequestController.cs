using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO.Request;
using SnapLink_Service.IService;
using System.Security.Claims;

namespace SnapLink_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WithdrawalRequestController : ControllerBase
    {
        private readonly IWithdrawalRequestService _withdrawalRequestService;

        public WithdrawalRequestController(IWithdrawalRequestService withdrawalRequestService)
        {
            _withdrawalRequestService = withdrawalRequestService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateWithdrawalRequest([FromBody] CreateWithdrawalRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new
                    {
                        Error = -1,
                        Message = "Invalid token or user not found",
                        Data = (object?)null
                    });
                }

                var result = await _withdrawalRequestService.CreateWithdrawalRequestAsync(request, userId);
                
                return Ok(new
                {
                    Error = 0,
                    Message = "Withdrawal request created successfully",
                    Data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    Error = -1,
                    Message = ex.Message,
                    Data = (object?)null
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateWithdrawalRequest: {ex.Message}");
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }

        [HttpPut("{withdrawalId}")]
        public async Task<IActionResult> UpdateWithdrawalRequest(int withdrawalId, [FromBody] UpdateWithdrawalRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new
                    {
                        Error = -1,
                        Message = "Invalid token or user not found",
                        Data = (object?)null
                    });
                }

                var result = await _withdrawalRequestService.UpdateWithdrawalRequestAsync(withdrawalId, request, userId);
                
                return Ok(new
                {
                    Error = 0,
                    Message = "Withdrawal request updated successfully",
                    Data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    Error = -1,
                    Message = ex.Message,
                    Data = (object?)null
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateWithdrawalRequest: {ex.Message}");
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }

        [HttpDelete("{withdrawalId}")]
        public async Task<IActionResult> CancelWithdrawalRequest(int withdrawalId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new
                    {
                        Error = -1,
                        Message = "Invalid token or user not found",
                        Data = (object?)null
                    });
                }

                var result = await _withdrawalRequestService.CancelWithdrawalRequestAsync(withdrawalId, userId);
                
                return Ok(new
                {
                    Error = 0,
                    Message = "Withdrawal request cancelled successfully",
                    Data = (object?)null
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    Error = -1,
                    Message = ex.Message,
                    Data = (object?)null
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CancelWithdrawalRequest: {ex.Message}");
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }

        [HttpGet("{withdrawalId}")]
        public async Task<IActionResult> GetWithdrawalRequest(int withdrawalId)
        {
            try
            {
                var result = await _withdrawalRequestService.GetWithdrawalRequestByIdAsync(withdrawalId);
                if (result == null)
                {
                    return NotFound(new
                    {
                        Error = -1,
                        Message = "Withdrawal request not found",
                        Data = (object?)null
                    });
                }

                return Ok(new
                {
                    Error = 0,
                    Message = "Withdrawal request retrieved successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetWithdrawalRequest: {ex.Message}");
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }

        [HttpGet("{withdrawalId}/detail")]
        public async Task<IActionResult> GetWithdrawalRequestDetail(int withdrawalId)
        {
            try
            {
                var result = await _withdrawalRequestService.GetWithdrawalRequestDetailAsync(withdrawalId);
                if (result == null)
                {
                    return NotFound(new
                    {
                        Error = -1,
                        Message = "Withdrawal request not found",
                        Data = (object?)null
                    });
                }

                return Ok(new
                {
                    Error = 0,
                    Message = "Withdrawal request detail retrieved successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetWithdrawalRequestDetail: {ex.Message}");
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserWithdrawalRequests([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new
                    {
                        Error = -1,
                        Message = "Invalid token or user not found",
                        Data = (object?)null
                    });
                }

                var result = await _withdrawalRequestService.GetUserWithdrawalRequestsAsync(userId, page, pageSize);
                
                return Ok(new
                {
                    Error = 0,
                    Message = "User withdrawal requests retrieved successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserWithdrawalRequests: {ex.Message}");
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> GetAllWithdrawalRequests([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? status = null)
        {
            try
            {
                var result = await _withdrawalRequestService.GetAllWithdrawalRequestsAsync(page, pageSize, status);
                
                return Ok(new
                {
                    Error = 0,
                    Message = "All withdrawal requests retrieved successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllWithdrawalRequests: {ex.Message}");
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> GetWithdrawalRequestsByStatus(string status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _withdrawalRequestService.GetWithdrawalRequestsByStatusAsync(status, page, pageSize);
                
                return Ok(new
                {
                    Error = 0,
                    Message = $"Withdrawal requests with status '{status}' retrieved successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetWithdrawalRequestsByStatus: {ex.Message}");
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }

        [HttpPut("{withdrawalId}/status")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> UpdateWithdrawalRequestStatus(int withdrawalId, [FromBody] UpdateWithdrawalStatusRequest request)
        {
            try
            {
                var moderatorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (moderatorIdClaim == null || !int.TryParse(moderatorIdClaim.Value, out int moderatorId))
                {
                    return Unauthorized(new
                    {
                        Error = -1,
                        Message = "Invalid token or moderator not found",
                        Data = (object?)null
                    });
                }

                object? result = null;
                string message = "";

                switch (request.Status.ToLower())
                {
                    case "approved":
                        if (string.IsNullOrEmpty(request.Message))
                        {
                            return BadRequest(new
                            {
                                Error = -1,
                                Message = "Bill image link is required when approving a withdrawal request",
                                Data = (object?)null
                            });
                        }
                        result = await _withdrawalRequestService.ApproveWithdrawalRequestAsync(withdrawalId, moderatorId, request.Message);
                        message = "Withdrawal request approved successfully with bill image link";
                        break;
                    case "rejected":
                        if (string.IsNullOrEmpty(request.Message))
                        {
                            return BadRequest(new
                            {
                                Error = -1,
                                Message = "Rejection reason is required when rejecting a withdrawal request",
                                Data = (object?)null
                            });
                        }
                        result = await _withdrawalRequestService.RejectWithdrawalRequestAsync(withdrawalId, request.Message, moderatorId);
                        message = "Withdrawal request rejected successfully";
                        break;
                    case "completed":
                        result = await _withdrawalRequestService.CompleteWithdrawalRequestAsync(withdrawalId, moderatorId);
                        message = "Withdrawal request completed successfully - funds deducted and transaction created";
                        break;
                    default:
                        return BadRequest(new
                        {
                            Error = -1,
                            Message = "Invalid status. Allowed values: approved, rejected, completed",
                            Data = (object?)null
                        });
                }
                
                return Ok(new
                {
                    Error = 0,
                    Message = message,
                    Data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    Error = -1,
                    Message = ex.Message,
                    Data = (object?)null
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateWithdrawalRequestStatus: {ex.Message}");
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }

        [HttpGet("limits")]
        public async Task<IActionResult> GetWithdrawalLimits()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new
                    {
                        Error = -1,
                        Message = "Invalid token or user not found",
                        Data = (object?)null
                    });
                }

                var minimumAmount = await _withdrawalRequestService.GetMinimumWithdrawalAmountAsync();
                var maximumAmount = await _withdrawalRequestService.GetMaximumWithdrawalAmountAsync(userId);
                
                return Ok(new
                {
                    Error = 0,
                    Message = "Withdrawal limits retrieved successfully",
                    Data = new
                    {
                        MinimumAmount = minimumAmount,
                        MaximumAmount = maximumAmount,
                        Currency = "VND"
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetWithdrawalLimits: {ex.Message}");
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }
    }

    public class UpdateWithdrawalStatusRequest
    {
        public string Status { get; set; } = string.Empty;
        public string? Message { get; set; } // For approved: bill image link, for rejected: rejection reason
    }
}
