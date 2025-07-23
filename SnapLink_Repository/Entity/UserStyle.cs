using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class UserStyle
{
    public int UserStyleId { get; set; }

    public int UserId { get; set; }

    public int StyleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Style Style { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
