namespace SnapLink_Model.DTO.Response
{
    public class ComplaintResponse
    {
        public int ComplaintId { get; set; }
        public int ReporterId { get; set; }
        public string? ReporterName { get; set; }
        public string? ReporterEmail { get; set; }
        public int ReportedUserId { get; set; }
        public string? ReportedUserName { get; set; }
        public string? ReportedUserEmail { get; set; }
        public int? BookingId { get; set; }
        public string? ComplaintType { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public int? AssignedModeratorId { get; set; }
        public string? AssignedModeratorName { get; set; }
        public string? ResolutionNotes { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ComplaintDetailResponse : ComplaintResponse
    {
        public BookingInfoDto? BookingInfo { get; set; }
        public UserInfoDto? ReporterInfo { get; set; }
        public UserInfoDto? ReportedUserInfo { get; set; }
        public ModeratorInfoDto? ModeratorInfo { get; set; }
    }

    public class ComplaintListResponse
    {
        public List<ComplaintResponse> Complaints { get; set; } = new List<ComplaintResponse>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class BookingInfoDto
    {
        public int BookingId { get; set; }
        public DateTime? StartDatetime { get; set; }
        public DateTime? EndDatetime { get; set; }
        public string? Status { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? LocationName { get; set; }
        public string? PhotographerName { get; set; }
    }

    public class UserInfoDto
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImage { get; set; }
    }

    public class ModeratorInfoDto
    {
        public int ModeratorId { get; set; }
        public string? ModeratorName { get; set; }
        public string? ModeratorEmail { get; set; }
        public string? AreasOfFocus { get; set; }
    }
}
