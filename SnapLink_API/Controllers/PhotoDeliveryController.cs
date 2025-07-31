using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Service.IService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SnapLink_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotoDeliveryController : ControllerBase
    {
        private readonly IPhotoDeliveryService _photoDeliveryService;

        public PhotoDeliveryController(IPhotoDeliveryService photoDeliveryService)
        {
            _photoDeliveryService = photoDeliveryService;
        }

        // GET: api/PhotoDelivery/booking/{bookingId}
        [HttpGet("booking/{bookingId}")]
        public async Task<ActionResult<ApiResponse<PhotoDeliveryResponse>>> GetPhotoDeliveryByBookingId(int bookingId)
        {
            try
            {
                var result = await _photoDeliveryService.GetPhotoDeliveryByBookingIdAsync(bookingId);
                return Ok(new ApiResponse<PhotoDeliveryResponse>
                {
                    Error = 0,
                    Message = "Photo delivery retrieved successfully",
                    Data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new ApiResponse<PhotoDeliveryResponse>
                {
                    Error = -1,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PhotoDeliveryResponse>
                {
                    Error = -1,
                    Message = "An error occurred while retrieving photo delivery",
                    Data = null
                });
            }
        }

        // GET: api/PhotoDelivery/{photoDeliveryId}
        [HttpGet("{photoDeliveryId}")]
        public async Task<ActionResult<ApiResponse<PhotoDeliveryResponse>>> GetPhotoDeliveryById(int photoDeliveryId)
        {
            try
            {
                var result = await _photoDeliveryService.GetPhotoDeliveryByIdAsync(photoDeliveryId);
                return Ok(new ApiResponse<PhotoDeliveryResponse>
                {
                    Error = 0,
                    Message = "Photo delivery retrieved successfully",
                    Data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new ApiResponse<PhotoDeliveryResponse>
                {
                    Error = -1,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PhotoDeliveryResponse>
                {
                    Error = -1,
                    Message = "An error occurred while retrieving photo delivery",
                    Data = null
                });
            }
        }

        // POST: api/PhotoDelivery
        [HttpPost]
        public async Task<ActionResult<ApiResponse<PhotoDeliveryResponse>>> CreatePhotoDelivery([FromBody] CreatePhotoDeliveryRequest request)
        {
            try
            {
                var result = await _photoDeliveryService.CreatePhotoDeliveryAsync(request);
                return CreatedAtAction(nameof(GetPhotoDeliveryById), new { photoDeliveryId = result.PhotoDeliveryId }, new ApiResponse<PhotoDeliveryResponse>
                {
                    Error = 0,
                    Message = "Photo delivery created successfully",
                    Data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<PhotoDeliveryResponse>
                {
                    Error = -1,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PhotoDeliveryResponse>
                {
                    Error = -1,
                    Message = "An error occurred while creating photo delivery",
                    Data = null
                });
            }
        }

        // PUT: api/PhotoDelivery/{photoDeliveryId}
        [HttpPut("{photoDeliveryId}")]
        public async Task<ActionResult<ApiResponse<PhotoDeliveryResponse>>> UpdatePhotoDelivery(int photoDeliveryId, [FromBody] UpdatePhotoDeliveryRequest request)
        {
            try
            {
                var result = await _photoDeliveryService.UpdatePhotoDeliveryAsync(photoDeliveryId, request);
                return Ok(new ApiResponse<PhotoDeliveryResponse>
                {
                    Error = 0,
                    Message = "Photo delivery updated successfully",
                    Data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new ApiResponse<PhotoDeliveryResponse>
                {
                    Error = -1,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PhotoDeliveryResponse>
                {
                    Error = -1,
                    Message = "An error occurred while updating photo delivery",
                    Data = null
                });
            }
        }

        // DELETE: api/PhotoDelivery/{photoDeliveryId}
        [HttpDelete("{photoDeliveryId}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeletePhotoDelivery(int photoDeliveryId)
        {
            try
            {
                var result = await _photoDeliveryService.DeletePhotoDeliveryAsync(photoDeliveryId);
                if (!result)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Error = -1,
                        Message = "Photo delivery not found",
                        Data = false
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Error = 0,
                    Message = "Photo delivery deleted successfully",
                    Data = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Error = -1,
                    Message = "An error occurred while deleting photo delivery",
                    Data = false
                });
            }
        }


        // GET: api/PhotoDelivery/photographer/{photographerId}
        [HttpGet("photographer/{photographerId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PhotoDeliveryResponse>>>> GetPhotoDeliveriesByPhotographerId(int photographerId)
        {
            try
            {
                var result = await _photoDeliveryService.GetPhotoDeliveriesByPhotographerIdAsync(photographerId);
                return Ok(new ApiResponse<IEnumerable<PhotoDeliveryResponse>>
                {
                    Error = 0,
                    Message = "Photo deliveries retrieved successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<PhotoDeliveryResponse>>
                {
                    Error = -1,
                    Message = "An error occurred while retrieving photo deliveries",
                    Data = null
                });
            }
        }

        // GET: api/PhotoDelivery/customer/{customerId}
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PhotoDeliveryResponse>>>> GetPhotoDeliveriesByCustomerId(int customerId)
        {
            try
            {
                var result = await _photoDeliveryService.GetPhotoDeliveriesByCustomerIdAsync(customerId);
                return Ok(new ApiResponse<IEnumerable<PhotoDeliveryResponse>>
                {
                    Error = 0,
                    Message = "Photo deliveries retrieved successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<PhotoDeliveryResponse>>
                {
                    Error = -1,
                    Message = "An error occurred while retrieving photo deliveries",
                    Data = null
                });
            }
        }

        // GET: api/PhotoDelivery/status/{status}
        [HttpGet("status/{status}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PhotoDeliveryResponse>>>> GetPhotoDeliveriesByStatus(string status)
        {
            try
            {
                var result = await _photoDeliveryService.GetPhotoDeliveriesByStatusAsync(status);
                return Ok(new ApiResponse<IEnumerable<PhotoDeliveryResponse>>
                {
                    Error = 0,
                    Message = "Photo deliveries retrieved successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<PhotoDeliveryResponse>>
                {
                    Error = -1,
                    Message = "An error occurred while retrieving photo deliveries",
                    Data = null
                });
            }
        }

        // GET: api/PhotoDelivery/pending
        [HttpGet("pending")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PhotoDeliveryResponse>>>> GetPendingPhotoDeliveries()
        {
            try
            {
                var result = await _photoDeliveryService.GetPendingPhotoDeliveriesAsync();
                return Ok(new ApiResponse<IEnumerable<PhotoDeliveryResponse>>
                {
                    Error = 0,
                    Message = "Pending photo deliveries retrieved successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<PhotoDeliveryResponse>>
                {
                    Error = -1,
                    Message = "An error occurred while retrieving pending photo deliveries",
                    Data = null
                });
            }
        }

        // GET: api/PhotoDelivery/expired
        [HttpGet("expired")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PhotoDeliveryResponse>>>> GetExpiredPhotoDeliveries()
        {
            try
            {
                var result = await _photoDeliveryService.GetExpiredPhotoDeliveriesAsync();
                return Ok(new ApiResponse<IEnumerable<PhotoDeliveryResponse>>
                {
                    Error = 0,
                    Message = "Expired photo deliveries retrieved successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<PhotoDeliveryResponse>>
                {
                    Error = -1,
                    Message = "An error occurred while retrieving expired photo deliveries",
                    Data = null
                });
            }
        }
    }

    // Helper class for API responses
    public class ApiResponse<T>
    {
        public int Error { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
} 