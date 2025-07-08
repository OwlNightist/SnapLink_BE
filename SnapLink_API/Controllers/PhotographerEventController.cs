using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO.Request;
using SnapLink_Service.IService;
using System;
using System.Threading.Tasks;

namespace SnapLink_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PhotographerEventController : ControllerBase
{
    private readonly IPhotographerEventService _eventService;

    public PhotographerEventController(IPhotographerEventService eventService)
    {
        _eventService = eventService;
    }

    /// <summary>
    /// Get all photographer events
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllEvents()
    {
        try
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <summary>
    /// Get photographer event by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEventById(int id)
    {
        try
        {
            var ev = await _eventService.GetEventByIdAsync(id);
            return Ok(ev);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new photographer event
    /// </summary>
    [HttpPost("{photographerId}")]
    public async Task<IActionResult> CreateEvent(int photographerId, [FromBody] CreatePhotographerEventRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var ev = await _eventService.CreateEventAsync(photographerId, request);
            return CreatedAtAction(nameof(GetEventById), new { id = ev.EventId }, ev);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <summary>
    /// Update photographer event
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdatePhotographerEventRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var ev = await _eventService.UpdateEventAsync(id, request);
            return Ok(ev);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete photographer event
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        try
        {
            var result = await _eventService.DeleteEventAsync(id);
            if (!result)
                return NotFound(new { message = $"Event with ID {id} not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }
} 