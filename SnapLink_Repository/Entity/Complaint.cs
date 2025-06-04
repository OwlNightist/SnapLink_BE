using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class Complaint
{
    public int ComplaintId { get; set; }

    public int ReporterId { get; set; }

    public int ReportedUserId { get; set; }

    public int? BookingId { get; set; }

    public string? ComplaintType { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public int? AssignedModeratorId { get; set; }

    public string? ResolutionNotes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Moderator? AssignedModerator { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual User ReportedUser { get; set; } = null!;

    public virtual User Reporter { get; set; } = null!;
}
