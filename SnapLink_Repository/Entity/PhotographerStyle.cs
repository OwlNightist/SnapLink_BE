using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class PhotographerStyle
{
    public int PhotographerStyleId { get; set; }

    public int PhotographerId { get; set; }

    public int StyleId { get; set; }

    public virtual Photographer Photographer { get; set; } = null!;

    public virtual Style Style { get; set; } = null!;
}
