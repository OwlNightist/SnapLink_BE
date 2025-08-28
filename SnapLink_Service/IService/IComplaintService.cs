using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService
{
    public interface IComplaintService
    {
        // Create complaint
        Task<ComplaintResponse> CreateComplaintAsync(CreateComplaintRequest request, int reporterId);
        
        // Get complaints
        Task<ComplaintResponse> GetComplaintByIdAsync(int complaintId);
        Task<ComplaintDetailResponse> GetComplaintDetailAsync(int complaintId);
        Task<ComplaintListResponse> GetAllComplaintsAsync(int page = 1, int pageSize = 10, string? status = null);
        Task<ComplaintListResponse> GetComplaintsByReporterAsync(int reporterId, int page = 1, int pageSize = 10);
        Task<ComplaintListResponse> GetComplaintsByReportedUserAsync(int reportedUserId, int page = 1, int pageSize = 10);
        Task<ComplaintListResponse> GetComplaintsByModeratorAsync(int moderatorId, int page = 1, int pageSize = 10);
        Task<ComplaintListResponse> GetComplaintsByStatusAsync(string status, int page = 1, int pageSize = 10);
        Task<ComplaintListResponse> GetComplaintsByTypeAsync(string complaintType, int page = 1, int pageSize = 10);
        Task<ComplaintListResponse> GetComplaintsByBookingIdAsync(int bookingId, int page = 1, int pageSize = 10);
        Task<ComplaintResponse?> GetComplaintByBookingIdAsync(int bookingId);
        
        // Update complaint
        Task<ComplaintResponse> UpdateComplaintAsync(int complaintId, UpdateComplaintRequest request);
        Task<ComplaintResponse> AssignModeratorAsync(int complaintId, AssignModeratorRequest request);
        Task<ComplaintResponse> ResolveComplaintAsync(int complaintId, ResolveComplaintRequest request, int moderatorId);
        Task<bool> UpdateComplaintStatusAsync(int complaintId, string status);
        
        // Delete complaint
        Task<bool> DeleteComplaintAsync(int complaintId);
        
        // Business logic
        Task<bool> CanUserFileComplaintAsync(int reporterId, int reportedUserId, int? bookingId = null);
        Task<IEnumerable<string>> GetComplaintTypesAsync();
        Task<IEnumerable<string>> GetComplaintStatusesAsync();
        Task<int> GetPendingComplaintsCountAsync();
        Task<int> GetComplaintsCountByModeratorAsync(int moderatorId);
    }
}
