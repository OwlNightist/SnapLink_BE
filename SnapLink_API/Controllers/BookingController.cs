using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO.Request;
using SnapLink_Service.IService;
using System.Security.Claims;

namespace SnapLink_API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
    {
        try
        {
            // Extract user ID from JWT token
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

            var result = await _bookingService.CreateBookingAsync(request, userId);
            
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
            Console.WriteLine($"Error in CreateBooking: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }



    [HttpGet("{bookingId}")]
    public async Task<IActionResult> GetBooking(int bookingId)
    {
        try
        {
            var result = await _bookingService.GetBookingByIdAsync(bookingId);
            
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
            Console.WriteLine($"Error in GetBooking: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserBookings(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _bookingService.GetUserBookingsAsync(userId, page, pageSize);
            
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
            Console.WriteLine($"Error in GetUserBookings: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    [HttpGet("photographer/{photographerId}")]
    public async Task<IActionResult> GetPhotographerBookings(int photographerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _bookingService.GetPhotographerBookingsAsync(photographerId, page, pageSize);
            
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
            Console.WriteLine($"Error in GetPhotographerBookings: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    [HttpPut("{bookingId}")]
    public async Task<IActionResult> UpdateBooking(int bookingId, [FromBody] UpdateBookingRequest request)
    {
        try
        {
            var result = await _bookingService.UpdateBookingAsync(bookingId, request);
            
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
            Console.WriteLine($"Error in UpdateBooking: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    [HttpPut("{bookingId}/cancel")]
    public async Task<IActionResult> CancelBooking(int bookingId)
    {
        try
        {
            var result = await _bookingService.CancelBookingAsync(bookingId);
            
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
            Console.WriteLine($"Error in CancelBooking: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    [HttpPut("{bookingId}/Complete")]
    public async Task<IActionResult> CompleteBooking(int bookingId)
    {
        try
        {
            var result = await _bookingService.CompleteBookingAsync(bookingId);
            
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
            Console.WriteLine($"Error in CompleteBooking: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    [HttpPut("{bookingId}/confirm")]
    public async Task<IActionResult> ConfirmBooking(int bookingId)
    {
        try
        {
            // Extract user ID from JWT token
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

            var result = await _bookingService.ConfirmBookingAsync(bookingId, userId);
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
            Console.WriteLine($"Error in ConfirmBooking: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    [HttpGet("availability/photographer/{photographerId}")]
    public async Task<IActionResult> CheckPhotographerAvailability(int photographerId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
    {
        try
        {
            var isAvailable = await _bookingService.IsPhotographerAvailableAsync(photographerId, startTime, endTime);
            
            return Ok(new
            {
                Error = 0,
                Message = "Availability check completed",
                Data = new
                {
                    PhotographerId = photographerId,
                    StartTime = startTime,
                    EndTime = endTime,
                    IsAvailable = isAvailable
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CheckPhotographerAvailability: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    [HttpGet("availability/location/{locationId}")]
    public async Task<IActionResult> CheckLocationAvailability(int locationId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
    {
        try
        {
            var isAvailable = await _bookingService.IsLocationAvailableAsync(locationId, startTime, endTime);
            
            return Ok(new
            {
                Error = 0,
                Message = "Availability check completed",
                Data = new
                {
                    LocationId = locationId,
                    StartTime = startTime,
                    EndTime = endTime,
                    IsAvailable = isAvailable
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CheckLocationAvailability: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    [HttpGet("availability/location/{locationId}/photographer/{photographerId}")]
    public async Task<IActionResult> CheckLocationAvailabilityForPhotographer(int locationId, int photographerId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
    {
        try
        {
            var isAvailable = await _bookingService.IsLocationAvailableForPhotographerAsync(locationId, photographerId, startTime, endTime);
            
            return Ok(new
            {
                Error = 0,
                Message = "Photographer-specific location availability check completed",
                Data = new
                {
                    LocationId = locationId,
                    PhotographerId = photographerId,
                    StartTime = startTime,
                    EndTime = endTime,
                    IsAvailable = isAvailable,
                    Note = isAvailable ? "Location is available for this photographer" : "Photographer already has a booking at this location during the selected time"
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CheckLocationAvailabilityForPhotographer: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    [HttpGet("location/{locationId}/photographers")]
    public async Task<IActionResult> GetPhotographersAtLocation(int locationId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
    {
        try
        {
            var photographers = await _bookingService.GetPhotographersAtLocationAsync(locationId, startTime, endTime);
            
            return Ok(new
            {
                Error = 0,
                Message = "Photographers at location retrieved successfully",
                Data = new
                {
                    LocationId = locationId,
                    StartTime = startTime,
                    EndTime = endTime,
                    PhotographerCount = photographers.Count(),
                    Photographers = photographers
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetPhotographersAtLocation: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    [HttpGet("calculate-price")]
    public async Task<IActionResult> CalculatePrice([FromQuery] int photographerId, [FromQuery] int? locationId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
    {
        try
        {
            var price = await _bookingService.CalculateBookingPriceAsync(photographerId, locationId, startTime, endTime);
            var duration = (endTime - startTime).TotalHours;
            
            return Ok(new
            {
                Error = 0,
                Message = "Price calculation completed",
                Data = new
                {
                    PhotographerId = photographerId,
                    LocationId = locationId,
                    LocationType = locationId.HasValue ? "Registered" : "External",
                    StartTime = startTime,
                    EndTime = endTime,
                    DurationHours = duration,
                    TotalPrice = price,
                    PricePerHour = duration > 0 ? price / (decimal)duration : 0,
                    LocationFee = locationId.HasValue ? 0 : 0, // Will be calculated based on location type
                    PhotographerFee = price // All fee goes to photographer for external locations
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CalculatePrice: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    [HttpPost("cleanup-expired")]
    public async Task<IActionResult> CleanupExpiredBookings()
    {
        try
        {
            var cancelledCount = await _bookingService.CancelExpiredPendingBookingsAsync();
            
            return Ok(new
            {
                Error = 0,
                Message = $"Cleanup completed. {cancelledCount} expired pending bookings cancelled.",
                Data = new
                {
                    CancelledCount = cancelledCount,
                    Timestamp = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CleanupExpiredBookings: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    [HttpPost("cleanup-all-pending")]
    public async Task<IActionResult> CleanupAllPendingBookings()
    {
        try
        {
            var cancelledCount = await _bookingService.CancelAllPendingBookingsAsync();
            
            return Ok(new
            {
                Error = 0,
                Message = $"Cleanup completed. {cancelledCount} pending bookings cancelled.",
                Data = new
                {
                    CancelledCount = cancelledCount,
                    Timestamp = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CleanupAllPendingBookings: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

} 