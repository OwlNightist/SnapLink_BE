using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class User
{
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public string? PhoneNumber { get; set; }

    public string? FullName { get; set; }

    public string? ProfileImage { get; set; }

    public string? Bio { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public string? Status { get; set; }
    public string? VerificationCode { get; set; }
    public bool IsVerified { get; set; } = false;

    public virtual ICollection<Administrator> Administrators { get; set; } = new List<Administrator>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Complaint> ComplaintReportedUsers { get; set; } = new List<Complaint>();

    public virtual ICollection<Complaint> ComplaintReporters { get; set; } = new List<Complaint>();

    public virtual ICollection<LocationOwner> LocationOwners { get; set; } = new List<LocationOwner>();

    public virtual ICollection<Messagess> MessagessRecipients { get; set; } = new List<Messagess>();

    public virtual ICollection<Messagess> MessagessSenders { get; set; } = new List<Messagess>();

    public virtual ICollection<Moderator> Moderators { get; set; } = new List<Moderator>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Photographer> Photographers { get; set; } = new List<Photographer>();

    public virtual ICollection<PremiumSubscription> PremiumSubscriptions { get; set; } = new List<PremiumSubscription>();

    public virtual ICollection<Transaction> FromTransactions { get; set; } = new List<Transaction>();
    public virtual ICollection<Transaction> ToTransactions { get; set; } = new List<Transaction>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    
    public virtual ICollection<UserStyle> UserStyles { get; set; } = new List<UserStyle>();
    
    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
}
