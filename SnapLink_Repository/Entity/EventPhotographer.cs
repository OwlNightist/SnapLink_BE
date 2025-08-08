using System;
using System.ComponentModel.DataAnnotations;

namespace SnapLink_Repository.Entity;

public partial class EventPhotographer
{
    public int EventPhotographerId { get; set; }

    public int EventId { get; set; }

    public int PhotographerId { get; set; }

    [MaxLength(30)]
    public string Status { get; set; } = "Applied"; // "Applied", "Approved", "Rejected", "Withdrawn"

    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ApprovedAt { get; set; }

    [MaxLength(500)]
    public string? RejectionReason { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? SpecialRate { get; set; } // Photographer's rate for this event

    // Navigation properties
    public virtual LocationEvent Event { get; set; } = null!;

    public virtual Photographer Photographer { get; set; } = null!;
}
