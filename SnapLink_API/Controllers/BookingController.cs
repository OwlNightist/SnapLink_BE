using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO.Request;
using SnapLink_Service.IService;

namespace SnapLink_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IPaymentService _paymentService;

    public BookingController(IBookingService bookingService, IPaymentService paymentService)
    {
        _bookingService = bookingService;
        _paymentService = paymentService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request, [FromQuery] int userId)
    {
        try
        {
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

    [HttpPut("{bookingId}/complete")]
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

    [HttpPost("{bookingId}/create-payment")]
    public async Task<IActionResult> CreatePaymentForBooking(int bookingId, [FromQuery] int userId, [FromQuery] string cancelUrl, [FromQuery] string returnUrl)
    {
        try
        {
            // Get booking details
            var bookingResult = await _bookingService.GetBookingByIdAsync(bookingId);
            
            if (bookingResult.Error != 0)
            {
                return BadRequest(bookingResult);
            }

            var booking = bookingResult.Data;
            
            // Verify booking belongs to user
            if (booking.UserId != userId)
            {
                return BadRequest(new
                {
                    Error = -1,
                    Message = "Booking does not belong to the specified user",
                    Data = (object?)null
                });
            }

            // Check if booking is in pending status
            if (booking.Status != "Pending")
            {
                return BadRequest(new
                {
                    Error = -1,
                    Message = "Payment can only be created for pending bookings",
                    Data = (object?)null
                });
            }

            // Create payment request
            var paymentRequest = new CreatePaymentLinkRequest
            {
                ProductName = $"Photography Session - {booking.PhotographerName} at {booking.LocationName}",
                Price = booking.TotalPrice,
                Description = $"Photography booking for {booking.DurationHours} hours on {booking.StartDatetime:MMM dd, yyyy}",
                CancelUrl = cancelUrl,
                ReturnUrl = returnUrl,
                PhotographerId = booking.PhotographerId,
                LocationId = booking.LocationId
            };

            var paymentResult = await _paymentService.CreatePaymentLinkAsync(paymentRequest, userId);
            
            if (paymentResult.Error == 0)
            {
                return Ok(new
                {
                    Error = 0,
                    Message = "Payment link created successfully",
                    Data = new
                    {
                        Booking = booking,
                        Payment = paymentResult.Data
                    }
                });
            }
            else
            {
                return BadRequest(paymentResult);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreatePaymentForBooking: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }
} 