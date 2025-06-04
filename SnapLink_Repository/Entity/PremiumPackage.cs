using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class PremiumPackage
{
    public int PackageId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public int? DurationDays { get; set; }

    public string? Features { get; set; }

    public string? ApplicableTo { get; set; }

    public virtual ICollection<PremiumSubscription> PremiumSubscriptions { get; set; } = new List<PremiumSubscription>();
}
