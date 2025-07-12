using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class Wallet
{
    public int WalletId { get; set; }

    public int UserId { get; set; }

    public decimal? Balance { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
} 