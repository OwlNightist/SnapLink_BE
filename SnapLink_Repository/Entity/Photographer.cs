using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class Photographer
{
    public int PhotographerId { get; set; }

    public int UserId { get; set; }

    public int? YearsExperience { get; set; }

    public string? Equipment { get; set; }

    public decimal? HourlyRate { get; set; }

    public string? AvailabilityStatus { get; set; }

    public decimal? Rating { get; set; }

    public decimal? RatingSum { get; set; }

    public int? RatingCount { get; set; }

    public bool? FeaturedStatus { get; set; }

    public string? VerificationStatus { get; set; }

    public string? Address { get; set; }

    public string? GoogleMapsAddress { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<PhotographerEvent> PhotographerEvents { get; set; } = new List<PhotographerEvent>();

    public virtual ICollection<PhotographerStyle> PhotographerStyles { get; set; } = new List<PhotographerStyle>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<WithdrawalRequest> WithdrawalRequests { get; set; } = new List<WithdrawalRequest>();

    public virtual ICollection<PhotographerEvent> PhotographerEvents { get; set; } = new List<PhotographerEvent>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();
}
