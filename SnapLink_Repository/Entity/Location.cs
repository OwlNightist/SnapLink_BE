using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class Location
{
    public int LocationId { get; set; }

    public int LocationOwnerId { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }

    public string? Amenities { get; set; }

    public decimal? HourlyRate { get; set; }

    public int? Capacity { get; set; }

    public bool? Indoor { get; set; }

    public bool? Outdoor { get; set; }

    public string? AvailabilityStatus { get; set; }

    public bool? FeaturedStatus { get; set; }

    public string? VerificationStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();

    public virtual ICollection<LocationImage> LocationImages { get; set; } = new List<LocationImage>();

    public virtual LocationOwner LocationOwner { get; set; } = null!;
}
