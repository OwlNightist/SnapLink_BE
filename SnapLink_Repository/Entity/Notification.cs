using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class Notification
{
    public int MotificationId { get; set; }

    public int UserId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? NotificationType { get; set; }

    public int? ReferenceId { get; set; }

    public bool? ReadStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
