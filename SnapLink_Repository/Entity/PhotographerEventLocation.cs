using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class PhotographerEventLocation
{
    public int EventLocationId { get; set; }

    public int EventId { get; set; }

    public int LocationId { get; set; }

    public virtual PhotographerEvent Event { get; set; } = null!;

    public virtual Location Location { get; set; } = null!;
}
