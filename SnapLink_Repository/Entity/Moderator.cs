using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class Moderator
{
    public int ModeratorId { get; set; }

    public int UserId { get; set; }

    public string? AreasOfFocus { get; set; }

    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

    public virtual User User { get; set; } = null!;
}
