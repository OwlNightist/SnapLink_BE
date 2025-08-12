using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Service.IService;
using System;
using System.Threading.Tasks;

namespace SnapLink_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AvailabilityController : ControllerBase
    {
        private readonly IAvailabilityService _availabilityService;

        public AvailabilityController(IAvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        /// <summary>
        /// Get all availabilities for a photographer
        /// </summary>
        [HttpGet("photographer/{photographerId}")]
        public async Task<IActionResult> GetPhotographerAvailabilities(int photographerId)
        {
            try
            {
                var availabilities = await _availabilityService.GetAvailabilitiesByPhotographerIdAsync(photographerId);
                return Ok(new
                {
                    Error = 0,
                    Message = "Availabilities retrieved successfully",
                    Data = availabilities
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }

        // /// <summary>
        // /// Get photographer availability with details
        // /// </summary>
        // [HttpGet("photographer/{photographerId}/details")]
        // public async Task<IActionResult> GetPhotographerAvailabilityDetails(int photographerId)
        // {
        //     try
        //     {
        //         var availability = await _availabilityService.GetPhotographerAvailabilityAsync(photographerId);
        //         return Ok(new
        //         {
        //             Error = 0,
        //             Message = "Photographer availability retrieved successfully",
        //             Data = availability
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, new
        //         {
        //             Error = -1,
        //             Message = "Internal server error",
        //             Data = (object?)null
        //         });
        //     }
        // }

        /// <summary>
        /// Get availabilities by day of week
        /// </summary>
        [HttpGet("day/{dayOfWeek}")]
        public async Task<IActionResult> GetAvailabilitiesByDay(DayOfWeek dayOfWeek)
        {
            try
            {
                var availabilities = await _availabilityService.GetAvailabilitiesByDayOfWeekAsync(dayOfWeek);
                return Ok(new
                {
                    Error = 0,
                    Message = "Availabilities retrieved successfully",
                    Data = availabilities
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }

        /// <summary>
        /// Get availability by ID
        /// </summary>
        [HttpGet("{availabilityId}")]
        public async Task<IActionResult> GetAvailabilityById(int availabilityId)
        {
            try
            {
                var availability = await _availabilityService.GetAvailabilityByIdAsync(availabilityId);
                if (availability == null)
                {
                    return NotFound(new
                    {
                        Error = 1,
                        Message = "Availability not found",
                        Data = (object?)null
                    });
                }

                return Ok(new
                {
                    Error = 0,
                    Message = "Availability retrieved successfully",
                    Data = availability
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }

        /// <summary>
        /// Create new availability
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAvailability([FromBody] CreateAvailabilityRequest request)
        {
            try
            {
                var availability = await _availabilityService.CreateAvailabilityAsync(request);
                return CreatedAtAction(nameof(GetAvailabilityById), new { availabilityId = availability.AvailabilityId }, new
                {
                    Error = 0,
                    Message = "Availability created successfully",
                    Data = availability
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    Error = 1,
                    Message = ex.Message,
                    Data = (object?)null
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    Error = 2,
                    Message = ex.Message,
                    Data = (object?)null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }

        /// <summary>
        /// Create bulk availabilities
        /// </summary>
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulkAvailability([FromBody] BulkAvailabilityRequest request)
        {
            try
            {
                var result = await _availabilityService.CreateBulkAvailabilityAsync(request);
                if (result)
                {
                    return Ok(new
                    {
                        Error = 0,
                        Message = "Bulk availabilities created successfully",
                        Data = (object?)null
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        Error = 1,
                        Message = "Failed to create bulk availabilities",
                        Data = (object?)null
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }

        /// <summary>
        /// Update availability
        /// </summary>
        [HttpPut("{availabilityId}")]
        public async Task<IActionResult> UpdateAvailability(int availabilityId, [FromBody] UpdateAvailabilityRequest request)
        {
            try
            {
                var availability = await _availabilityService.UpdateAvailabilityAsync(availabilityId, request);
                return Ok(new
                {
                    Error = 0,
                    Message = "Availability updated successfully",
                    Data = availability
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    Error = 1,
                    Message = ex.Message,
                    Data = (object?)null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }

        /// <summary>
        /// Delete availability
        /// </summary>
        [HttpDelete("{availabilityId}")]
        public async Task<IActionResult> DeleteAvailability(int availabilityId)
        {
            try
            {
                var result = await _availabilityService.DeleteAvailabilityAsync(availabilityId);
                if (result)
                {
                    return Ok(new
                    {
                        Error = 0,
                        Message = "Availability deleted successfully",
                        Data = (object?)null
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        Error = 1,
                        Message = "Availability not found",
                        Data = (object?)null
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }

        /// <summary>
        /// Delete all availabilities for a photographer
        /// </summary>
        [HttpDelete("photographer/{photographerId}")]
        public async Task<IActionResult> DeletePhotographerAvailabilities(int photographerId)
        {
            try
            {
                var result = await _availabilityService.DeleteAvailabilitiesByPhotographerIdAsync(photographerId);
                return Ok(new
                {
                    Error = 0,
                    Message = "Photographer availabilities deleted successfully",
                    Data = (object?)null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }

        /// <summary>
        /// Check availability for a specific time slot
        /// </summary>
        [HttpPost("check")]
        public async Task<IActionResult> CheckAvailability([FromBody] CheckAvailabilityRequest request)
        {
            try
            {
                var result = await _availabilityService.CheckAvailabilityAsync(request);
                return Ok(new
                {
                    Error = 0,
                    Message = "Availability check completed",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }

        /// <summary>
        /// Update availability status
        /// </summary>
        [HttpPatch("{availabilityId}/status")]
        public async Task<IActionResult> UpdateAvailabilityStatus(int availabilityId, [FromBody] string status)
        {
            try
            {
                var result = await _availabilityService.UpdateAvailabilityStatusAsync(availabilityId, status);
                if (result)
                {
                    return Ok(new
                    {
                        Error = 0,
                        Message = "Availability status updated successfully",
                        Data = (object?)null
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        Error = 1,
                        Message = "Availability not found",
                        Data = (object?)null
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }

        /// <summary>
        /// Get available photographers by time
        /// </summary>
        [HttpGet("available-photographers")]
        public async Task<IActionResult> GetAvailablePhotographersByTime([FromQuery] DayOfWeek dayOfWeek, [FromQuery] TimeSpan startTime, [FromQuery] TimeSpan endTime)
        {
            try
            {
                var availabilities = await _availabilityService.GetAvailablePhotographersByTimeAsync(dayOfWeek, startTime, endTime);
                return Ok(new
                {
                    Error = 0,
                    Message = "Available photographers retrieved successfully",
                    Data = availabilities
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }

        /// <summary>
        /// Get available time slots for a photographer on a specific date
        /// This considers their registered availability and existing bookings
        /// </summary>
        [HttpGet("photographer/{photographerId}/available-slots")]
        public async Task<IActionResult> GetAvailableTimeSlots(int photographerId, [FromQuery] DateTime date)
        {
            try
            {
                // Validate photographer ID
                if (photographerId <= 0)
                {
                    return BadRequest(new
                    {
                        Error = 1,
                        Message = "Invalid photographer ID",
                        Data = (object?)null
                    });
                }

                // Validate date (should not be in the past for future bookings)
                if (date.Date < DateTime.Today)
                {
                    return BadRequest(new
                    {
                        Error = 2,
                        Message = "Date cannot be in the past",
                        Data = (object?)null
                    });
                }

                var availableSlots = await _availabilityService.GetAvailableTimeSlotsAsync(photographerId, date);
                return Ok(new
                {
                    Error = 0,
                    Message = "Available time slots retrieved successfully",
                    Data = availableSlots
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = -1,
                    Message = "Internal server error",
                    Data = (object?)null
                });
            }
        }
    }
} 