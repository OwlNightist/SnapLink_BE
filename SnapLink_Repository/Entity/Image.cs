using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class Image
{
    public int Id { get; set; }

    public string Url { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public string? Caption { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? PhotographerId { get; set; }

    public int? LocationId { get; set; }

    public int? PhotographerEventId { get; set; }

    public virtual Location? Location { get; set; }

    public virtual Photographer? Photographer { get; set; }

    public virtual PhotographerEvent? PhotographerEvent { get; set; }
}
