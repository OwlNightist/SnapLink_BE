using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class LocationOwner
{
    public int LocationOwnerId { get; set; }

    public int UserId { get; set; }

    public string? BusinessName { get; set; }

    public string? BusinessAddress { get; set; }

    public string? BusinessRegistrationNumber { get; set; }

    public string? VerificationStatus { get; set; }

    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();

    public virtual User User { get; set; } = null!;
}
