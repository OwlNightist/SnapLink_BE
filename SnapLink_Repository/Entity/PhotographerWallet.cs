using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class PhotographerWallet
{
    public int PhotographerWalletId { get; set; }

    public int PhotographerId { get; set; }

    public decimal? Balance { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Photographer Photographer { get; set; } = null!;
}
