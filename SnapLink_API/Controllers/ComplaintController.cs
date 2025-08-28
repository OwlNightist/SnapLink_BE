using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Service.IService;
using System.Security.Claims;

namespace SnapLink_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ComplaintController : ControllerBase
    {
        private readonly IComplaintService _complaintService;

        public ComplaintController(IComplaintService complaintService)
        {
            _complaintService = complaintService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("Invalid user ID in token");
        }

        /// <summary>
        /// Create a new complaint
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ComplaintResponse>> CreateComplaint([FromBody] CreateComplaintRequest request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var complaint = await _complaintService.CreateComplaintAsync(request, currentUserId);
                return Ok(complaint);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while creating the complaint", details = ex.Message });
            }
        }

        /// <summary>
        /// Get complaint by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ComplaintResponse>> GetComplaint(int id)
        {
            try
            {
                var complaint = await _complaintService.GetComplaintByIdAsync(id);
                return Ok(complaint);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving the complaint", details = ex.Message });
            }
        }

        /// <summary>
        /// Get detailed complaint information by ID
        /// </summary>
        [HttpGet("{id}/detail")]
        public async Task<ActionResult<ComplaintDetailResponse>> GetComplaintDetail(int id)
        {
            try
            {
                var complaint = await _complaintService.GetComplaintDetailAsync(id);
                return Ok(complaint);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving the complaint details", details = ex.Message });
            }
        }

        /// <summary>
        /// Get all complaints with optional filtering
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<ComplaintListResponse>> GetAllComplaints(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null)
        {
            try
            {
                var complaints = await _complaintService.GetAllComplaintsAsync(page, pageSize, status);
                return Ok(complaints);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving complaints", details = ex.Message });
            }
        }

        /// <summary>
        /// Get complaints filed by current user
        /// </summary>
        [HttpGet("my-reports")]
        public async Task<ActionResult<ComplaintListResponse>> GetMyReports(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var complaints = await _complaintService.GetComplaintsByReporterAsync(currentUserId, page, pageSize);
                return Ok(complaints);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving your reports", details = ex.Message });
            }
        }

        /// <summary>
        /// Get complaints filed against current user
        /// </summary>
        [HttpGet("against-me")]
        public async Task<ActionResult<ComplaintListResponse>> GetComplaintsAgainstMe(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var complaints = await _complaintService.GetComplaintsByReportedUserAsync(currentUserId, page, pageSize);
                return Ok(complaints);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving complaints against you", details = ex.Message });
            }
        }

        /// <summary>
        /// Get complaints by reporter ID (Admin/Moderator only)
        /// </summary>
        [HttpGet("by-reporter/{reporterId}")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<ComplaintListResponse>> GetComplaintsByReporter(
            int reporterId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var complaints = await _complaintService.GetComplaintsByReporterAsync(reporterId, page, pageSize);
                return Ok(complaints);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving complaints", details = ex.Message });
            }
        }

        /// <summary>
        /// Get complaints by reported user ID (Admin/Moderator only)
        /// </summary>
        [HttpGet("by-reported-user/{reportedUserId}")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<ComplaintListResponse>> GetComplaintsByReportedUser(
            int reportedUserId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var complaints = await _complaintService.GetComplaintsByReportedUserAsync(reportedUserId, page, pageSize);
                return Ok(complaints);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving complaints", details = ex.Message });
            }
        }

        /// <summary>
        /// Get complaints assigned to a moderator
        /// </summary>
        [HttpGet("by-moderator/{moderatorId}")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<ComplaintListResponse>> GetComplaintsByModerator(
            int moderatorId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var complaints = await _complaintService.GetComplaintsByModeratorAsync(moderatorId, page, pageSize);
                return Ok(complaints);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving complaints", details = ex.Message });
            }
        }

        /// <summary>
        /// Get complaints by status
        /// </summary>
        [HttpGet("by-status/{status}")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<ComplaintListResponse>> GetComplaintsByStatus(
            string status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var complaints = await _complaintService.GetComplaintsByStatusAsync(status, page, pageSize);
                return Ok(complaints);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving complaints", details = ex.Message });
            }
        }

        /// <summary>
        /// Get complaints by type
        /// </summary>
        [HttpGet("by-type/{type}")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<ComplaintListResponse>> GetComplaintsByType(
            string type,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var complaints = await _complaintService.GetComplaintsByTypeAsync(type, page, pageSize);
                return Ok(complaints);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving complaints", details = ex.Message });
            }
        }

        /// <summary>
        /// Get complaint by booking ID
        /// </summary>
        [HttpGet("by-booking/{bookingId}")]
        public async Task<ActionResult<ComplaintResponse>> GetComplaintByBookingId(int bookingId)
        {
            try
            {
                var complaint = await _complaintService.GetComplaintByBookingIdAsync(bookingId);
                if (complaint == null)
                {
                    return NotFound(new { error = "No complaint found for this booking" });
                }
                return Ok(complaint);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving the complaint", details = ex.Message });
            }
        }

        /// <summary>
        /// Update complaint (Admin/Moderator only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<ComplaintResponse>> UpdateComplaint(int id, [FromBody] UpdateComplaintRequest request)
        {
            try
            {
                var complaint = await _complaintService.UpdateComplaintAsync(id, request);
                return Ok(complaint);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the complaint", details = ex.Message });
            }
        }

        /// <summary>
        /// Assign moderator to complaint
        /// </summary>
        [HttpPost("{id}/assign-moderator")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<ComplaintResponse>> AssignModerator(int id, [FromBody] AssignModeratorRequest request)
        {
            try
            {
                var complaint = await _complaintService.AssignModeratorAsync(id, request);
                return Ok(complaint);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while assigning moderator", details = ex.Message });
            }
        }

        /// <summary>
        /// Resolve complaint (Moderator only)
        /// </summary>
        [HttpPost("{id}/resolve")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<ComplaintResponse>> ResolveComplaint(int id, [FromBody] ResolveComplaintRequest request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var complaint = await _complaintService.ResolveComplaintAsync(id, request, currentUserId);
                return Ok(complaint);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while resolving the complaint", details = ex.Message });
            }
        }

        /// <summary>
        /// Update complaint status
        /// </summary>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult> UpdateComplaintStatus(int id, [FromBody] string status)
        {
            try
            {
                var success = await _complaintService.UpdateComplaintStatusAsync(id, status);
                if (!success)
                {
                    return NotFound(new { error = "Complaint not found" });
                }
                return Ok(new { message = "Status updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the status", details = ex.Message });
            }
        }

        /// <summary>
        /// Delete complaint (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteComplaint(int id)
        {
            try
            {
                var success = await _complaintService.DeleteComplaintAsync(id);
                if (!success)
                {
                    return NotFound(new { error = "Complaint not found" });
                }
                return Ok(new { message = "Complaint deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deleting the complaint", details = ex.Message });
            }
        }

        /// <summary>
        /// Get available complaint types
        /// </summary>
        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<string>>> GetComplaintTypes()
        {
            try
            {
                var types = await _complaintService.GetComplaintTypesAsync();
                return Ok(types);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving complaint types", details = ex.Message });
            }
        }

        /// <summary>
        /// Get available complaint statuses
        /// </summary>
        [HttpGet("statuses")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<IEnumerable<string>>> GetComplaintStatuses()
        {
            try
            {
                var statuses = await _complaintService.GetComplaintStatusesAsync();
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving complaint statuses", details = ex.Message });
            }
        }

        /// <summary>
        /// Get pending complaints count
        /// </summary>
        [HttpGet("pending-count")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<int>> GetPendingComplaintsCount()
        {
            try
            {
                var count = await _complaintService.GetPendingComplaintsCountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving pending complaints count", details = ex.Message });
            }
        }

        /// <summary>
        /// Get complaints count by moderator
        /// </summary>
        [HttpGet("moderator-count/{moderatorId}")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<int>> GetComplaintsCountByModerator(int moderatorId)
        {
            try
            {
                var count = await _complaintService.GetComplaintsCountByModeratorAsync(moderatorId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving complaints count", details = ex.Message });
            }
        }
    }
}
