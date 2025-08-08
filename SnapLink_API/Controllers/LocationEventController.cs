using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO.Request;
using SnapLink_Service.IService;
using System.Security.Claims;

namespace SnapLink_API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LocationEventController : ControllerBase
{
    private readonly ILocationEventService _locationEventService;

    public LocationEventController(ILocationEventService locationEventService)
    {
        _locationEventService = locationEventService;
    }

    #region Event Management

    /// <summary>
    /// Create a new promotional event
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] CreateLocationEventRequest request)
    {
        try
        {
            var result = await _locationEventService.CreateEventAsync(request);
            return Ok(new
            {
                Error = 0,
                Message = "Event created successfully",
                Data = result
            });
        }
        catch (ArgumentException ex)
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
            Console.WriteLine($"Error in CreateEvent: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Get event by ID
    /// </summary>
    [HttpGet("{eventId}")]
    public async Task<IActionResult> GetEventById(int eventId)
    {
        try
        {
            var result = await _locationEventService.GetEventByIdAsync(eventId);
            return Ok(new
            {
                Error = 0,
                Message = "Event retrieved successfully",
                Data = result
            });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new
            {
                Error = -1,
                Message = ex.Message,
                Data = (object?)null
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetEventById: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Get detailed event information
    /// </summary>
    [HttpGet("{eventId}/detail")]
    public async Task<IActionResult> GetEventDetail(int eventId)
    {
        try
        {
            var result = await _locationEventService.GetEventDetailAsync(eventId);
            return Ok(new
            {
                Error = 0,
                Message = "Event details retrieved successfully",
                Data = result
            });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new
            {
                Error = -1,
                Message = ex.Message,
                Data = (object?)null
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetEventDetail: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Get all events
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllEvents()
    {
        try
        {
            var result = await _locationEventService.GetAllEventsAsync();
            return Ok(new
            {
                Error = 0,
                Message = "Events retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAllEvents: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Get events by location
    /// </summary>
    [HttpGet("location/{locationId}")]
    public async Task<IActionResult> GetEventsByLocation(int locationId)
    {
        try
        {
            var result = await _locationEventService.GetEventsByLocationAsync(locationId);
            return Ok(new
            {
                Error = 0,
                Message = "Events retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetEventsByLocation: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Get active events
    /// </summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveEvents()
    {
        try
        {
            var result = await _locationEventService.GetActiveEventsAsync();
            return Ok(new
            {
                Error = 0,
                Message = "Active events retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetActiveEvents: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Get upcoming events
    /// </summary>
    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcomingEvents()
    {
        try
        {
            var result = await _locationEventService.GetUpcomingEventsAsync();
            return Ok(new
            {
                Error = 0,
                Message = "Upcoming events retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetUpcomingEvents: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Update event
    /// </summary>
    [HttpPut("{eventId}")]
    public async Task<IActionResult> UpdateEvent(int eventId, [FromBody] UpdateLocationEventRequest request)
    {
        try
        {
            var result = await _locationEventService.UpdateEventAsync(eventId, request);
            return Ok(new
            {
                Error = 0,
                Message = "Event updated successfully",
                Data = result
            });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new
            {
                Error = -1,
                Message = ex.Message,
                Data = (object?)null
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateEvent: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Delete event
    /// </summary>
    [HttpDelete("{eventId}")]
    public async Task<IActionResult> DeleteEvent(int eventId)
    {
        try
        {
            var result = await _locationEventService.DeleteEventAsync(eventId);
            if (result)
            {
                return Ok(new
                {
                    Error = 0,
                    Message = "Event deleted successfully",
                    Data = (object?)null
                });
            }
            else
            {
                return NotFound(new
                {
                    Error = -1,
                    Message = "Event not found",
                    Data = (object?)null
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteEvent: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Update event status
    /// </summary>
    [HttpPatch("{eventId}/status")]
    public async Task<IActionResult> UpdateEventStatus(int eventId, [FromBody] string status)
    {
        try
        {
            var result = await _locationEventService.UpdateEventStatusAsync(eventId, status);
            if (result)
            {
                return Ok(new
                {
                    Error = 0,
                    Message = "Event status updated successfully",
                    Data = (object?)null
                });
            }
            else
            {
                return NotFound(new
                {
                    Error = -1,
                    Message = "Event not found",
                    Data = (object?)null
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateEventStatus: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    #endregion

    #region Event Applications

    /// <summary>
    /// Apply to event as photographer
    /// </summary>
    [HttpPost("apply")]
    public async Task<IActionResult> ApplyToEvent([FromBody] EventApplicationRequest request)
    {
        try
        {
            var result = await _locationEventService.ApplyToEventAsync(request);
            return Ok(new
            {
                Error = 0,
                Message = "Application submitted successfully",
                Data = result
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                Error = -1,
                Message = ex.Message,
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ApplyToEvent: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Respond to photographer application
    /// </summary>
    [HttpPost("respond-application")]
    public async Task<IActionResult> RespondToApplication([FromBody] EventApplicationResponseRequest request)
    {
        try
        {
            var result = await _locationEventService.RespondToApplicationAsync(request);
            return Ok(new
            {
                Error = 0,
                Message = "Application response submitted successfully",
                Data = result
            });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new
            {
                Error = -1,
                Message = ex.Message,
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error in RespondToApplication: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Get event applications
    /// </summary>
    [HttpGet("{eventId}/applications")]
    public async Task<IActionResult> GetEventApplications(int eventId)
    {
        try
        {
            var result = await _locationEventService.GetEventApplicationsAsync(eventId);
            return Ok(new
            {
                Error = 0,
                Message = "Applications retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetEventApplications: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Get photographer applications
    /// </summary>
    [HttpGet("photographer/{photographerId}/applications")]
    public async Task<IActionResult> GetPhotographerApplications(int photographerId)
    {
        try
        {
            var result = await _locationEventService.GetPhotographerApplicationsAsync(photographerId);
            return Ok(new
            {
                Error = 0,
                Message = "Applications retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetPhotographerApplications: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Withdraw application
    /// </summary>
    [HttpDelete("{eventId}/photographer/{photographerId}/withdraw")]
    public async Task<IActionResult> WithdrawApplication(int eventId, int photographerId)
    {
        try
        {
            var result = await _locationEventService.WithdrawApplicationAsync(eventId, photographerId);
            if (result)
            {
                return Ok(new
                {
                    Error = 0,
                    Message = "Application withdrawn successfully",
                    Data = (object?)null
                });
            }
            else
            {
                return BadRequest(new
                {
                    Error = -1,
                    Message = "Unable to withdraw application",
                    Data = (object?)null
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in WithdrawApplication: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Get approved photographers for event
    /// </summary>
    [HttpGet("{eventId}/approved-photographers")]
    public async Task<IActionResult> GetApprovedPhotographers(int eventId)
    {
        try
        {
            var result = await _locationEventService.GetApprovedPhotographersAsync(eventId);
            return Ok(new
            {
                Error = 0,
                Message = "Approved photographers retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetApprovedPhotographers: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    #endregion

    #region Event Bookings

    /// <summary>
    /// Create event booking
    /// </summary>
    [HttpPost("booking")]
    public async Task<IActionResult> CreateEventBooking([FromBody] EventBookingRequest request)
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

            // Set the user ID from the token
            request.UserId = userId;

            var result = await _locationEventService.CreateEventBookingAsync(request);
            return Ok(new
            {
                Error = 0,
                Message = "Event booking created successfully",
                Data = result
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                Error = -1,
                Message = ex.Message,
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateEventBooking: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Get event booking by ID
    /// </summary>
    [HttpGet("booking/{eventBookingId}")]
    public async Task<IActionResult> GetEventBooking(int eventBookingId)
    {
        try
        {
            var result = await _locationEventService.GetEventBookingByIdAsync(eventBookingId);
            return Ok(new
            {
                Error = 0,
                Message = "Event booking retrieved successfully",
                Data = result
            });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new
            {
                Error = -1,
                Message = ex.Message,
                Data = (object?)null
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetEventBooking: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Get event bookings
    /// </summary>
    [HttpGet("{eventId}/bookings")]
    public async Task<IActionResult> GetEventBookings(int eventId)
    {
        try
        {
            var result = await _locationEventService.GetEventBookingsAsync(eventId);
            return Ok(new
            {
                Error = 0,
                Message = "Event bookings retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetEventBookings: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Get user event bookings
    /// </summary>
    [HttpGet("user/{userId}/bookings")]
    public async Task<IActionResult> GetUserEventBookings(int userId)
    {
        try
        {
            var result = await _locationEventService.GetUserEventBookingsAsync(userId);
            return Ok(new
            {
                Error = 0,
                Message = "User event bookings retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetUserEventBookings: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Cancel event booking
    /// </summary>
    [HttpDelete("booking/{eventBookingId}")]
    public async Task<IActionResult> CancelEventBooking(int eventBookingId)
    {
        try
        {
            var result = await _locationEventService.CancelEventBookingAsync(eventBookingId);
            if (result)
            {
                return Ok(new
                {
                    Error = 0,
                    Message = "Event booking cancelled successfully",
                    Data = (object?)null
                });
            }
            else
            {
                return NotFound(new
                {
                    Error = -1,
                    Message = "Event booking not found",
                    Data = (object?)null
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CancelEventBooking: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    #endregion

    #region Event Statistics and Discovery

    /// <summary>
    /// Get event statistics
    /// </summary>
    [HttpGet("{eventId}/statistics")]
    public async Task<IActionResult> GetEventStatistics(int eventId)
    {
        try
        {
            var result = await _locationEventService.GetEventStatisticsAsync(eventId);
            return Ok(new
            {
                Error = 0,
                Message = "Event statistics retrieved successfully",
                Data = result
            });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new
            {
                Error = -1,
                Message = ex.Message,
                Data = (object?)null
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetEventStatistics: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Get event list
    /// </summary>
    [HttpGet("list")]
    public async Task<IActionResult> GetEventList()
    {
        try
        {
            var result = await _locationEventService.GetEventListAsync();
            return Ok(new
            {
                Error = 0,
                Message = "Event list retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetEventList: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Get events by status
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetEventsByStatus(string status)
    {
        try
        {
            var result = await _locationEventService.GetEventsByStatusAsync(status);
            return Ok(new
            {
                Error = 0,
                Message = "Events retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetEventsByStatus: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Get events by date range
    /// </summary>
    [HttpGet("date-range")]
    public async Task<IActionResult> GetEventsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        try
        {
            var result = await _locationEventService.GetEventsByDateRangeAsync(startDate, endDate);
            return Ok(new
            {
                Error = 0,
                Message = "Events retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetEventsByDateRange: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Search events
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchEvents([FromQuery] string searchTerm)
    {
        try
        {
            var result = await _locationEventService.SearchEventsAsync(searchTerm);
            return Ok(new
            {
                Error = 0,
                Message = "Search results retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SearchEvents: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Get featured events
    /// </summary>
    [HttpGet("featured")]
    public async Task<IActionResult> GetFeaturedEvents()
    {
        try
        {
            var result = await _locationEventService.GetFeaturedEventsAsync();
            return Ok(new
            {
                Error = 0,
                Message = "Featured events retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetFeaturedEvents: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// Get events nearby
    /// </summary>
    [HttpGet("nearby")]
    public async Task<IActionResult> GetEventsNearby([FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] double radiusKm = 10.0)
    {
        try
        {
            var result = await _locationEventService.GetEventsNearbyAsync(latitude, longitude, radiusKm);
            return Ok(new
            {
                Error = 0,
                Message = "Nearby events retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetEventsNearby: {ex.Message}");
            return StatusCode(500, new
            {
                Error = -1,
                Message = "Internal server error",
                Data = (object?)null
            });
        }
    }

    #endregion
}
