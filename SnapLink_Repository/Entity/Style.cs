using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class Style
{
    public int StyleId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<PhotographerStyle> PhotographerStyles { get; set; } = new List<PhotographerStyle>();
}
