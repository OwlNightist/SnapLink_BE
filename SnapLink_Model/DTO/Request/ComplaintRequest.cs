using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Request
{
    public class CreateComplaintRequest
    {
        [Required]
        public int ReportedUserId { get; set; }

        public int? BookingId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ComplaintType { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateComplaintRequest
    {
        [MaxLength(50)]
        public string? ComplaintType { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }

        public int? AssignedModeratorId { get; set; }

        [MaxLength(500)]
        public string? ResolutionNotes { get; set; }
    }

    public class AssignModeratorRequest
    {
        [Required]
        public int UserId { get; set; }
    }

    public class ResolveComplaintRequest
    {
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty; // "Resolved", "Rejected"

        [Required]
        [MaxLength(500)]
        public string ResolutionNotes { get; set; } = string.Empty;
    }
}
