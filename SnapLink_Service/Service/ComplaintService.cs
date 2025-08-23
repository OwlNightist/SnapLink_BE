using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Repository.Entity;
using SnapLink_Repository.Repository;
using SnapLink_Service.IService;

namespace SnapLink_Service.Service
{
    public class ComplaintService : IComplaintService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ComplaintService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ComplaintResponse> CreateComplaintAsync(CreateComplaintRequest request, int reporterId)
        {
            // Validate if user can file complaint
            var canFile = await CanUserFileComplaintAsync(reporterId, request.ReportedUserId, request.BookingId);
            if (!canFile)
            {
                throw new InvalidOperationException("User cannot file complaint against this user for this booking");
            }

            // Check if reporter and reported user exist
            var reporter = await _unitOfWork.UserRepository.GetByIdAsync(reporterId);
            if (reporter == null)
            {
                throw new ArgumentException("Reporter not found");
            }

            var reportedUser = await _unitOfWork.UserRepository.GetByIdAsync(request.ReportedUserId);
            if (reportedUser == null)
            {
                throw new ArgumentException("Reported user not found");
            }

            // If booking is provided, validate it exists
            if (request.BookingId.HasValue)
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(request.BookingId.Value);
                if (booking == null)
                {
                    throw new ArgumentException("Booking not found");
                }
            }

            var complaint = new Complaint
            {
                ReporterId = reporterId,
                ReportedUserId = request.ReportedUserId,
                BookingId = request.BookingId,
                ComplaintType = request.ComplaintType,
                Description = request.Description,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ComplaintRepository.AddAsync(complaint);
            await _unitOfWork.SaveChangesAsync();

            return await GetComplaintByIdAsync(complaint.ComplaintId);
        }

        public async Task<ComplaintResponse> GetComplaintByIdAsync(int complaintId)
        {
            var complaints = await _unitOfWork.ComplaintRepository.GetAsync(
                filter: c => c.ComplaintId == complaintId,
                includeProperties: "Reporter,ReportedUser,AssignedModerator"
            );

            var complaint = complaints.FirstOrDefault();
            if (complaint == null)
            {
                throw new ArgumentException("Complaint not found");
            }

            return MapToComplaintResponse(complaint);
        }

        public async Task<ComplaintDetailResponse> GetComplaintDetailAsync(int complaintId)
        {
            var complaints = await _unitOfWork.ComplaintRepository.GetAsync(
                filter: c => c.ComplaintId == complaintId,
                includeProperties: "Reporter,ReportedUser,AssignedModerator,Booking,Booking.Location,Booking.Photographer,Booking.Photographer.User"
            );

            var complaint = complaints.FirstOrDefault();
            if (complaint == null)
            {
                throw new ArgumentException("Complaint not found");
            }

            return MapToComplaintDetailResponse(complaint);
        }

        public async Task<ComplaintListResponse> GetAllComplaintsAsync(int page = 1, int pageSize = 10, string? status = null)
        {
            var query = await _unitOfWork.ComplaintRepository.GetAsync(
                filter: status != null ? c => c.Status == status : null,
                includeProperties: "Reporter,ReportedUser,AssignedModerator",
                orderBy: q => q.OrderByDescending(c => c.CreatedAt)
            );

            var totalCount = query.Count();
            var complaints = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new ComplaintListResponse
            {
                Complaints = complaints.Select(MapToComplaintResponse).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<ComplaintListResponse> GetComplaintsByReporterAsync(int reporterId, int page = 1, int pageSize = 10)
        {
            var query = await _unitOfWork.ComplaintRepository.GetAsync(
                filter: c => c.ReporterId == reporterId,
                includeProperties: "Reporter,ReportedUser,AssignedModerator",
                orderBy: q => q.OrderByDescending(c => c.CreatedAt)
            );

            var totalCount = query.Count();
            var complaints = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new ComplaintListResponse
            {
                Complaints = complaints.Select(MapToComplaintResponse).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<ComplaintListResponse> GetComplaintsByReportedUserAsync(int reportedUserId, int page = 1, int pageSize = 10)
        {
            var query = await _unitOfWork.ComplaintRepository.GetAsync(
                filter: c => c.ReportedUserId == reportedUserId,
                includeProperties: "Reporter,ReportedUser,AssignedModerator",
                orderBy: q => q.OrderByDescending(c => c.CreatedAt)
            );

            var totalCount = query.Count();
            var complaints = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new ComplaintListResponse
            {
                Complaints = complaints.Select(MapToComplaintResponse).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<ComplaintListResponse> GetComplaintsByModeratorAsync(int moderatorId, int page = 1, int pageSize = 10)
        {
            var query = await _unitOfWork.ComplaintRepository.GetAsync(
                filter: c => c.AssignedModeratorId == moderatorId,
                includeProperties: "Reporter,ReportedUser,AssignedModerator",
                orderBy: q => q.OrderByDescending(c => c.CreatedAt)
            );

            var totalCount = query.Count();
            var complaints = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new ComplaintListResponse
            {
                Complaints = complaints.Select(MapToComplaintResponse).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<ComplaintListResponse> GetComplaintsByStatusAsync(string status, int page = 1, int pageSize = 10)
        {
            var query = await _unitOfWork.ComplaintRepository.GetAsync(
                filter: c => c.Status == status,
                includeProperties: "Reporter,ReportedUser,AssignedModerator",
                orderBy: q => q.OrderByDescending(c => c.CreatedAt)
            );

            var totalCount = query.Count();
            var complaints = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new ComplaintListResponse
            {
                Complaints = complaints.Select(MapToComplaintResponse).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<ComplaintListResponse> GetComplaintsByTypeAsync(string complaintType, int page = 1, int pageSize = 10)
        {
            var query = await _unitOfWork.ComplaintRepository.GetAsync(
                filter: c => c.ComplaintType == complaintType,
                includeProperties: "Reporter,ReportedUser,AssignedModerator",
                orderBy: q => q.OrderByDescending(c => c.CreatedAt)
            );

            var totalCount = query.Count();
            var complaints = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new ComplaintListResponse
            {
                Complaints = complaints.Select(MapToComplaintResponse).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<ComplaintResponse> UpdateComplaintAsync(int complaintId, UpdateComplaintRequest request)
        {
            var complaint = await _unitOfWork.ComplaintRepository.GetByIdAsync(complaintId);
            if (complaint == null)
            {
                throw new ArgumentException("Complaint not found");
            }

            if (!string.IsNullOrEmpty(request.ComplaintType))
                complaint.ComplaintType = request.ComplaintType;

            if (!string.IsNullOrEmpty(request.Description))
                complaint.Description = request.Description;

            if (!string.IsNullOrEmpty(request.Status))
                complaint.Status = request.Status;

            if (request.AssignedModeratorId.HasValue)
            {
                // Validate moderator exists
                var moderator = await _unitOfWork.ModeratorRepository.GetByIdAsync(request.AssignedModeratorId.Value);
                if (moderator == null)
                {
                    throw new ArgumentException("Moderator not found");
                }
                complaint.AssignedModeratorId = request.AssignedModeratorId;
            }

            if (!string.IsNullOrEmpty(request.ResolutionNotes))
                complaint.ResolutionNotes = request.ResolutionNotes;

            complaint.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ComplaintRepository.Update(complaint);
            await _unitOfWork.SaveChangesAsync();

            return await GetComplaintByIdAsync(complaintId);
        }

        public async Task<ComplaintResponse> AssignModeratorAsync(int complaintId, AssignModeratorRequest request)
        {
            var complaint = await _unitOfWork.ComplaintRepository.GetByIdAsync(complaintId);
            if (complaint == null)
            {
                throw new ArgumentException("Complaint not found");
            }

            // Validate moderator exists
            var moderator = await _unitOfWork.ModeratorRepository.GetByIdAsync(request.ModeratorId);
            if (moderator == null)
            {
                throw new ArgumentException("Moderator not found");
            }

            complaint.AssignedModeratorId = request.ModeratorId;
            complaint.Status = "Assigned";
            complaint.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ComplaintRepository.Update(complaint);
            await _unitOfWork.SaveChangesAsync();

            return await GetComplaintByIdAsync(complaintId);
        }

        public async Task<ComplaintResponse> ResolveComplaintAsync(int complaintId, ResolveComplaintRequest request, int moderatorId)
        {
            var complaint = await _unitOfWork.ComplaintRepository.GetByIdAsync(complaintId);
            if (complaint == null)
            {
                throw new ArgumentException("Complaint not found");
            }

            // Validate that the moderator is assigned to this complaint or is an admin
            if (complaint.AssignedModeratorId != moderatorId)
            {
                throw new UnauthorizedAccessException("Moderator is not assigned to this complaint");
            }

            complaint.Status = request.Status;
            complaint.ResolutionNotes = request.ResolutionNotes;
            complaint.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ComplaintRepository.Update(complaint);
            await _unitOfWork.SaveChangesAsync();

            return await GetComplaintByIdAsync(complaintId);
        }

        public async Task<bool> UpdateComplaintStatusAsync(int complaintId, string status)
        {
            var complaint = await _unitOfWork.ComplaintRepository.GetByIdAsync(complaintId);
            if (complaint == null)
            {
                return false;
            }

            complaint.Status = status;
            complaint.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ComplaintRepository.Update(complaint);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteComplaintAsync(int complaintId)
        {
            var complaint = await _unitOfWork.ComplaintRepository.GetByIdAsync(complaintId);
            if (complaint == null)
            {
                return false;
            }

            _unitOfWork.ComplaintRepository.Remove(complaint);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CanUserFileComplaintAsync(int reporterId, int reportedUserId, int? bookingId = null)
        {
            // Users cannot complain about themselves
            if (reporterId == reportedUserId)
            {
                return false;
            }

            // Check if booking exists and involves both users
            if (bookingId.HasValue)
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId.Value);
                if (booking == null)
                {
                    return false;
                }

                // Check if both users are involved in the booking
                var isReporterInvolved = booking.UserId == reporterId || booking.PhotographerId == reporterId;
                var isReportedUserInvolved = booking.UserId == reportedUserId || booking.PhotographerId == reportedUserId;

                if (!isReporterInvolved || !isReportedUserInvolved)
                {
                    return false;
                }

                // Check if there's already a complaint for this booking between these users
                var existingComplaints = await _unitOfWork.ComplaintRepository.GetAsync(
                    filter: c => c.ReporterId == reporterId && 
                                c.ReportedUserId == reportedUserId && 
                                c.BookingId == bookingId.Value
                );

                if (existingComplaints.Any())
                {
                    return false; // Complaint already exists
                }
            }

            return true;
        }

        public async Task<IEnumerable<string>> GetComplaintTypesAsync()
        {
            return new List<string>
            {
                "No Show",
                "Poor Service",
                "Inappropriate Behavior",
                "Payment Issue",
                "Quality Issue",
                "Communication Issue",
                "Late Arrival",
                "Cancellation Issue",
                "Equipment Issue",
                "Safety Concern",
                "Other"
            };
        }

        public async Task<IEnumerable<string>> GetComplaintStatusesAsync()
        {
            return new List<string>
            {
                "Pending",
                "Assigned",
                "Under Review",
                "Resolved",
                "Rejected",
                "Closed"
            };
        }

        public async Task<int> GetPendingComplaintsCountAsync()
        {
            var pendingComplaints = await _unitOfWork.ComplaintRepository.GetAsync(
                filter: c => c.Status == "Pending"
            );

            return pendingComplaints.Count();
        }

        public async Task<int> GetComplaintsCountByModeratorAsync(int moderatorId)
        {
            var complaints = await _unitOfWork.ComplaintRepository.GetAsync(
                filter: c => c.AssignedModeratorId == moderatorId
            );

            return complaints.Count();
        }

        private ComplaintResponse MapToComplaintResponse(Complaint complaint)
        {
            return new ComplaintResponse
            {
                ComplaintId = complaint.ComplaintId,
                ReporterId = complaint.ReporterId,
                ReporterName = complaint.Reporter?.FullName,
                ReporterEmail = complaint.Reporter?.Email,
                ReportedUserId = complaint.ReportedUserId,
                ReportedUserName = complaint.ReportedUser?.FullName,
                ReportedUserEmail = complaint.ReportedUser?.Email,
                BookingId = complaint.BookingId,
                ComplaintType = complaint.ComplaintType,
                Description = complaint.Description,
                Status = complaint.Status,
                AssignedModeratorId = complaint.AssignedModeratorId,
                AssignedModeratorName = complaint.AssignedModerator?.User?.FullName,
                ResolutionNotes = complaint.ResolutionNotes,
                CreatedAt = complaint.CreatedAt,
                UpdatedAt = complaint.UpdatedAt
            };
        }

        private ComplaintDetailResponse MapToComplaintDetailResponse(Complaint complaint)
        {
            var response = new ComplaintDetailResponse
            {
                ComplaintId = complaint.ComplaintId,
                ReporterId = complaint.ReporterId,
                ReporterName = complaint.Reporter?.FullName,
                ReporterEmail = complaint.Reporter?.Email,
                ReportedUserId = complaint.ReportedUserId,
                ReportedUserName = complaint.ReportedUser?.FullName,
                ReportedUserEmail = complaint.ReportedUser?.Email,
                BookingId = complaint.BookingId,
                ComplaintType = complaint.ComplaintType,
                Description = complaint.Description,
                Status = complaint.Status,
                AssignedModeratorId = complaint.AssignedModeratorId,
                AssignedModeratorName = complaint.AssignedModerator?.User?.FullName,
                ResolutionNotes = complaint.ResolutionNotes,
                CreatedAt = complaint.CreatedAt,
                UpdatedAt = complaint.UpdatedAt
            };

            // Map booking info if available
            if (complaint.Booking != null)
            {
                response.BookingInfo = new BookingInfoDto
                {
                    BookingId = complaint.Booking.BookingId,
                    StartDatetime = complaint.Booking.StartDatetime,
                    EndDatetime = complaint.Booking.EndDatetime,
                    Status = complaint.Booking.Status,
                    TotalPrice = complaint.Booking.TotalPrice,
                    LocationName = complaint.Booking.Location?.Name,
                    PhotographerName = complaint.Booking.Photographer?.User?.FullName
                };
            }

            // Map reporter info
            if (complaint.Reporter != null)
            {
                response.ReporterInfo = new UserInfoDto
                {
                    UserId = complaint.Reporter.UserId,
                    UserName = complaint.Reporter.UserName,
                    Email = complaint.Reporter.Email,
                    FullName = complaint.Reporter.FullName,
                    PhoneNumber = complaint.Reporter.PhoneNumber,
                    ProfileImage = complaint.Reporter.ProfileImage
                };
            }

            // Map reported user info
            if (complaint.ReportedUser != null)
            {
                response.ReportedUserInfo = new UserInfoDto
                {
                    UserId = complaint.ReportedUser.UserId,
                    UserName = complaint.ReportedUser.UserName,
                    Email = complaint.ReportedUser.Email,
                    FullName = complaint.ReportedUser.FullName,
                    PhoneNumber = complaint.ReportedUser.PhoneNumber,
                    ProfileImage = complaint.ReportedUser.ProfileImage
                };
            }

            // Map moderator info
            if (complaint.AssignedModerator != null)
            {
                response.ModeratorInfo = new ModeratorInfoDto
                {
                    ModeratorId = complaint.AssignedModerator.ModeratorId,
                    ModeratorName = complaint.AssignedModerator.User?.FullName,
                    ModeratorEmail = complaint.AssignedModerator.User?.Email,
                    AreasOfFocus = complaint.AssignedModerator.AreasOfFocus
                };
            }

            return response;
        }
    }
}
