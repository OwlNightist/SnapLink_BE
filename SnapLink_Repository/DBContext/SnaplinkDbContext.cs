using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SnapLink_Repository.Entity;

namespace SnapLink_Repository.DBContext;

public partial class SnaplinkDbContext : DbContext
{
    public SnaplinkDbContext()
    {
    }

    public SnaplinkDbContext(DbContextOptions<SnaplinkDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Administrator> Administrators { get; set; }

    public virtual DbSet<Advertisement> Advertisements { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Complaint> Complaints { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<LocationOwner> LocationOwners { get; set; }

    public virtual DbSet<Messagess> Messagesses { get; set; }

    public virtual DbSet<Conversation> Conversations { get; set; }

    public virtual DbSet<ConversationParticipant> ConversationParticipants { get; set; }

    public virtual DbSet<Moderator> Moderators { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Photographer> Photographers { get; set; }

    public virtual DbSet<PhotographerStyle> PhotographerStyles { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    public virtual DbSet<PremiumPackage> PremiumPackages { get; set; }

    public virtual DbSet<PremiumSubscription> PremiumSubscriptions { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Style> Styles { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserStyle> UserStyles { get; set; }

    public virtual DbSet<WithdrawalRequest> WithdrawalRequests { get; set; }

    public virtual DbSet<Availability> Availabilities { get; set; }

    public virtual DbSet<PhotoDelivery> PhotoDeliveries { get; set; }

    public virtual DbSet<DeviceInfo> DeviceInfos { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<LocationEvent> LocationEvents { get; set; }

    public virtual DbSet<EventPhotographer> EventPhotographers { get; set; }

    public virtual DbSet<EventBooking> EventBookings { get; set; }
    
    public virtual DbSet<Rating> Ratings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnectionString());

    private string GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
        return configuration["ConnectionStrings:DefaultConnection"];
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User entity configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).ValueGeneratedOnAdd();
            entity.Property(e => e.UserName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.ProfileImage).HasMaxLength(500);
            entity.Property(e => e.Bio).HasMaxLength(1000);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.VerificationCode).HasMaxLength(10);
            entity.Property(e => e.PasswordResetCode).HasMaxLength(10);
            
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.UserName).IsUnique();
        });

        // Role entity configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId);
            entity.Property(e => e.RoleId).ValueGeneratedOnAdd();
            entity.Property(e => e.RoleName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.RoleDescription).HasMaxLength(500);
            
            entity.HasIndex(e => e.RoleName).IsUnique();
        });

        // UserRole entity configuration (many-to-many relationship)
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId);
            entity.Property(e => e.UserRoleId).ValueGeneratedOnAdd();
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Photographer entity configuration
        modelBuilder.Entity<Photographer>(entity =>
        {
            entity.HasKey(e => e.PhotographerId);
            entity.Property(e => e.PhotographerId).ValueGeneratedOnAdd();
            entity.Property(e => e.Equipment).HasMaxLength(1000);
            entity.Property(e => e.AvailabilityStatus).HasMaxLength(50);
            entity.Property(e => e.VerificationStatus).HasMaxLength(50);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.GoogleMapsAddress).HasMaxLength(500);
            entity.Property(e => e.Latitude).HasPrecision(10, 8);
            entity.Property(e => e.Longitude).HasPrecision(11, 8);
            entity.Property(e => e.HourlyRate).HasPrecision(10, 2);
            entity.Property(e => e.Rating).HasPrecision(3, 2);
            entity.Property(e => e.RatingSum).HasPrecision(10, 2);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Photographers)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Location entity configuration
        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId);
            entity.Property(e => e.LocationId).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Amenities).HasMaxLength(1000);
            entity.Property(e => e.HourlyRate).HasPrecision(10, 2);
            entity.Property(e => e.AvailabilityStatus).HasMaxLength(50);
            entity.Property(e => e.VerificationStatus).HasMaxLength(50);
            entity.Property(e => e.LocationType).HasMaxLength(50);
            entity.Property(e => e.ExternalPlaceId).HasMaxLength(100);
            entity.Property(e => e.Latitude).HasPrecision(10, 8);
            entity.Property(e => e.Longitude).HasPrecision(11, 8);
            
            entity.HasOne(e => e.LocationOwner)
                .WithMany(lo => lo.Locations)
                .HasForeignKey(e => e.LocationOwnerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // LocationOwner entity configuration
        modelBuilder.Entity<LocationOwner>(entity =>
        {
            entity.HasKey(e => e.LocationOwnerId);
            entity.Property(e => e.LocationOwnerId).ValueGeneratedOnAdd();
            entity.Property(e => e.BusinessName).HasMaxLength(200);
            entity.Property(e => e.BusinessAddress).HasMaxLength(500);
            entity.Property(e => e.BusinessRegistrationNumber).HasMaxLength(100);
            entity.Property(e => e.VerificationStatus).HasMaxLength(50);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.LocationOwners)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Booking entity configuration
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId);
            entity.Property(e => e.BookingId).ValueGeneratedOnAdd();
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.SpecialRequests).HasMaxLength(1000);
            entity.Property(e => e.TotalPrice).HasPrecision(10, 2);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Photographer)
                .WithMany(p => p.Bookings)
                .HasForeignKey(e => e.PhotographerId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Location)
                .WithMany(l => l.Bookings)
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Payment entity configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId);
            entity.Property(e => e.PaymentId).ValueGeneratedOnAdd();
            entity.Property(e => e.TotalAmount).HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.Currency).HasMaxLength(3).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ExternalTransactionId).HasMaxLength(100);
            entity.Property(e => e.Method).HasMaxLength(50);
            entity.Property(e => e.Note).HasMaxLength(500);
            
            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(e => e.BookingId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Transaction entity configuration
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId);
            entity.Property(e => e.TransactionId).ValueGeneratedOnAdd();
            entity.Property(e => e.Amount).HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.Currency).HasMaxLength(3).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.Note).HasMaxLength(500);
            
            entity.HasOne(e => e.ReferencePayment)
                .WithMany(p => p.Transactions)
                .HasForeignKey(e => e.ReferencePaymentId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.FromUser)
                .WithMany(u => u.FromTransactions)
                .HasForeignKey(e => e.FromUserId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.ToUser)
                .WithMany(u => u.ToTransactions)
                .HasForeignKey(e => e.ToUserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Image entity configuration
        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Url).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Caption).HasMaxLength(500);
            entity.Property(e => e.IsPrimary).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Images)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Photographer)
                .WithMany(p => p.Images)
                .HasForeignKey(e => e.PhotographerId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Location)
                .WithMany(l => l.Images)
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.LocationEvent)
                .WithMany(le => le.Images)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Style entity configuration
        modelBuilder.Entity<Style>(entity =>
        {
            entity.HasKey(e => e.StyleId);
            entity.Property(e => e.StyleId).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // UserStyle entity configuration (many-to-many relationship)
        modelBuilder.Entity<UserStyle>(entity =>
        {
            entity.HasKey(e => e.UserStyleId);
            entity.Property(e => e.UserStyleId).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).IsRequired();
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserStyles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Style)
                .WithMany(s => s.UserStyles)
                .HasForeignKey(e => e.StyleId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // PhotographerStyle entity configuration (many-to-many relationship)
        modelBuilder.Entity<PhotographerStyle>(entity =>
        {
            entity.HasKey(e => e.PhotographerStyleId);
            entity.Property(e => e.PhotographerStyleId).ValueGeneratedOnAdd();
            
            entity.HasOne(e => e.Photographer)
                .WithMany(p => p.PhotographerStyles)
                .HasForeignKey(e => e.PhotographerId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Style)
                .WithMany(s => s.PhotographerStyles)
                .HasForeignKey(e => e.StyleId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Review entity configuration
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId);
            entity.Property(e => e.ReviewId).ValueGeneratedOnAdd();
            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.Rating).IsRequired();
            entity.Property(e => e.RevieweeType).HasMaxLength(50);
            entity.Property(e => e.RevieweeId).IsRequired();
            entity.Property(e => e.ReviewerId).IsRequired();
            
            entity.HasOne(e => e.Booking)
                .WithMany(b => b.Reviews)
                .HasForeignKey(e => e.BookingId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Rating entity configuration
        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.RatingId);
            entity.Property(e => e.RatingId).ValueGeneratedOnAdd();
            entity.Property(e => e.BookingId).IsRequired();
            entity.Property(e => e.ReviewerUserId).IsRequired();
            entity.Property(e => e.Score).IsRequired();
            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.UpdatedAt);
            
            entity.HasOne(e => e.ReviewerUser)
                .WithMany(u => u.RatingsAuthored)
                .HasForeignKey(e => e.ReviewerUserId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Photographer)
                .WithMany(p => p.Ratings)
                .HasForeignKey(e => e.PhotographerId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Location)
                .WithMany(l => l.Ratings)
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Booking)
                .WithMany(b => b.Ratings)
                .HasForeignKey(e => e.BookingId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Availability entity configuration
        modelBuilder.Entity<Availability>(entity =>
        {
            entity.HasKey(e => e.AvailabilityId);
            entity.Property(e => e.AvailabilityId).ValueGeneratedOnAdd();
            entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
            entity.Property(e => e.DayOfWeek).IsRequired();
            entity.Property(e => e.StartTime).IsRequired();
            entity.Property(e => e.EndTime).IsRequired();
            
            entity.HasOne(e => e.Photographer)
                .WithMany(p => p.Availabilities)
                .HasForeignKey(e => e.PhotographerId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // PhotoDelivery entity configuration
        modelBuilder.Entity<PhotoDelivery>(entity =>
        {
            entity.HasKey(e => e.PhotoDeliveryId);
            entity.Property(e => e.PhotoDeliveryId).ValueGeneratedOnAdd();
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.DeliveryMethod).HasMaxLength(50).IsRequired();
            entity.Property(e => e.DriveLink).HasMaxLength(500);
            entity.Property(e => e.DriveFolderName).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(500);
            
            entity.HasOne(e => e.Booking)
                .WithOne(b => b.PhotoDelivery)
                .HasForeignKey<PhotoDelivery>(e => e.BookingId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // LocationEvent entity configuration
        modelBuilder.Entity<LocationEvent>(entity =>
        {
            entity.HasKey(e => e.EventId);
            entity.Property(e => e.EventId).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.StartDate).IsRequired();
            entity.Property(e => e.EndDate).IsRequired();
            entity.Property(e => e.DiscountedPrice).HasPrecision(10, 2);
            entity.Property(e => e.OriginalPrice).HasPrecision(10, 2);
            entity.Property(e => e.MaxPhotographers).IsRequired();
            entity.Property(e => e.MaxBookingsPerSlot).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(30).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            entity.HasOne(e => e.Location)
                .WithMany(l => l.LocationEvents)
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // EventPhotographer entity configuration
        modelBuilder.Entity<EventPhotographer>(entity =>
        {
            entity.HasKey(e => e.EventPhotographerId);
            entity.Property(e => e.EventPhotographerId).ValueGeneratedOnAdd();
            entity.Property(e => e.Status).HasMaxLength(30).IsRequired();
            entity.Property(e => e.AppliedAt).IsRequired();
            entity.Property(e => e.ApprovedAt);
            entity.Property(e => e.RejectionReason).HasMaxLength(500);
            entity.Property(e => e.SpecialRate).HasPrecision(10, 2);
            
            entity.HasOne(e => e.Event)
                .WithMany()
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Photographer)
                .WithMany(p => p.EventPhotographers)
                .HasForeignKey(e => e.PhotographerId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // EventBooking entity configuration
        modelBuilder.Entity<EventBooking>(entity =>
        {
            entity.HasKey(e => e.EventBookingId);
            entity.Property(e => e.EventBookingId).ValueGeneratedOnAdd();
            entity.Property(e => e.EventPrice).HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            
            entity.HasOne(e => e.Event)
                .WithMany()
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Booking)
                .WithOne(b => b.EventBooking)
                .HasForeignKey<EventBooking>(e => e.BookingId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.EventPhotographer)
                .WithMany()
                .HasForeignKey(e => e.EventPhotographerId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Notification entity configuration
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.MotificationId);
            entity.Property(e => e.MotificationId).ValueGeneratedOnAdd();
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Content).HasMaxLength(1000);
            entity.Property(e => e.NotificationType).HasMaxLength(50);
            entity.Property(e => e.ReferenceId);
            entity.Property(e => e.ReadStatus);
            entity.Property(e => e.CreatedAt);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Wallet entity configuration
        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.WalletId);
            entity.Property(e => e.WalletId).ValueGeneratedOnAdd();
            entity.Property(e => e.Balance).HasPrecision(10, 2);
            entity.Property(e => e.UpdatedAt);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Wallets)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // PremiumPackage entity configuration
        modelBuilder.Entity<PremiumPackage>(entity =>
        {
            entity.HasKey(e => e.PackageId);
            entity.Property(e => e.PackageId).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.DurationDays);
            entity.Property(e => e.Features).HasMaxLength(1000);
            entity.Property(e => e.ApplicableTo).HasMaxLength(100);
        });

        // PremiumSubscription entity configuration
        modelBuilder.Entity<PremiumSubscription>(entity =>
        {
            entity.HasKey(e => e.PremiumSubscriptionId);
            entity.Property(e => e.PremiumSubscriptionId).ValueGeneratedOnAdd();
            entity.Property(e => e.StartDate);
            entity.Property(e => e.EndDate);
            entity.Property(e => e.Status).HasMaxLength(50);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.PremiumSubscriptions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Photographer)
                .WithMany(p => p.PremiumSubscriptions)
                .HasForeignKey(e => e.PhotographerId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Location)
                .WithMany(l => l.PremiumSubscriptions)
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Package)
                .WithMany()
                .HasForeignKey(e => e.PackageId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Payment)
                .WithMany(p => p.PremiumSubscriptions)
                .HasForeignKey(e => e.PaymentId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Complaint entity configuration
        modelBuilder.Entity<Complaint>(entity =>
        {
            entity.HasKey(e => e.ComplaintId);
            entity.Property(e => e.ComplaintId).ValueGeneratedOnAdd();
            entity.Property(e => e.ComplaintType).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.ResolutionNotes).HasMaxLength(1000);
            
            entity.HasOne(e => e.Reporter)
                .WithMany(u => u.ComplaintReporters)
                .HasForeignKey(e => e.ReporterId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.ReportedUser)
                .WithMany(u => u.ComplaintReportedUsers)
                .HasForeignKey(e => e.ReportedUserId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Booking)
                .WithMany(b => b.Complaints)
                .HasForeignKey(e => e.BookingId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.AssignedModerator)
                .WithMany()
                .HasForeignKey(e => e.AssignedModeratorId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Conversation entity configuration
        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.ConversationId);
            entity.Property(e => e.ConversationId).ValueGeneratedOnAdd();
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.UpdatedAt);
        });

        // ConversationParticipant entity configuration
        modelBuilder.Entity<ConversationParticipant>(entity =>
        {
            entity.HasKey(e => e.ConversationParticipantId);
            entity.Property(e => e.ConversationParticipantId).ValueGeneratedOnAdd();
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.JoinedAt);
            entity.Property(e => e.LeftAt);
            entity.Property(e => e.IsActive).IsRequired();
            
            entity.HasOne(e => e.Conversation)
                .WithMany(c => c.Participants)
                .HasForeignKey(e => e.ConversationId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.User)
                .WithMany(u => u.ConversationParticipants)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Messagess entity configuration
        modelBuilder.Entity<Messagess>(entity =>
        {
            entity.HasKey(e => e.MessageId);
            entity.Property(e => e.MessageId).ValueGeneratedOnAdd();
            entity.Property(e => e.Content).HasMaxLength(2000);
            entity.Property(e => e.MessageType).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.ReadAt);
            
            entity.HasOne(e => e.Sender)
                .WithMany(u => u.MessagessSenders)
                .HasForeignKey(e => e.SenderId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Recipient)
                .WithMany(u => u.MessagessRecipients)
                .HasForeignKey(e => e.RecipientId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(e => e.ConversationId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Administrator entity configuration
        modelBuilder.Entity<Administrator>(entity =>
        {
            entity.HasKey(e => e.AdminId);
            entity.Property(e => e.AdminId).ValueGeneratedOnAdd();
            entity.Property(e => e.AccessLevel).HasMaxLength(50);
            entity.Property(e => e.Department).HasMaxLength(100);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Administrators)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Moderator entity configuration
        modelBuilder.Entity<Moderator>(entity =>
        {
            entity.HasKey(e => e.ModeratorId);
            entity.Property(e => e.ModeratorId).ValueGeneratedOnAdd();
            entity.Property(e => e.AreasOfFocus).HasMaxLength(200);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Moderators)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Advertisement entity configuration
        modelBuilder.Entity<Advertisement>(entity =>
        {
            entity.HasKey(e => e.AdvertisementId);
            entity.Property(e => e.AdvertisementId).ValueGeneratedOnAdd();
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.StartDate);
            entity.Property(e => e.EndDate);
            entity.Property(e => e.Cost).HasPrecision(10, 2);
            
            entity.HasOne(e => e.Location)
                .WithMany(l => l.Advertisements)
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.Payment)
                .WithMany(p => p.Advertisements)
                .HasForeignKey(e => e.PaymentId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // DeviceInfo entity configuration
        modelBuilder.Entity<DeviceInfo>(entity =>
        {
            entity.HasKey(e => e.DeviceInfoId);
            entity.Property(e => e.DeviceInfoId).ValueGeneratedOnAdd();
            entity.Property(e => e.DeviceType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Brand).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Model).HasMaxLength(100).IsRequired();
            entity.Property(e => e.OperatingSystem).HasMaxLength(20);
            entity.Property(e => e.OsVersion).HasMaxLength(20);
            entity.Property(e => e.ScreenResolution).HasMaxLength(20);
            entity.Property(e => e.CameraResolution).HasMaxLength(20);
            entity.Property(e => e.StorageCapacity).HasMaxLength(100);
            entity.Property(e => e.BatteryCapacity).HasMaxLength(100);
            entity.Property(e => e.Features).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.LastUsedAt);
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.UpdatedAt);
            
            entity.HasOne(e => e.Photographer)
                .WithMany(p => p.DeviceInfos)
                .HasForeignKey(e => e.PhotographerId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Device entity configuration (for push notifications)
        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.DeviceId);
            entity.Property(e => e.DeviceId).ValueGeneratedOnAdd();
            entity.Property(e => e.ExpoPushToken).HasMaxLength(255).IsRequired();
            entity.Property(e => e.DeviceType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.DeviceId_External).HasMaxLength(255);
            entity.Property(e => e.DeviceName).HasMaxLength(100);
            entity.Property(e => e.AppVersion).HasMaxLength(50);
            entity.Property(e => e.OsVersion).HasMaxLength(50);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.LastUsedAt);
            
            entity.HasOne(e => e.User)
                .WithMany(p => p.Devices)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            entity.HasIndex(e => e.UserId).HasDatabaseName("IX_Devices_UserId");
            entity.HasIndex(e => e.ExpoPushToken).HasDatabaseName("IX_Devices_ExpoPushToken");
            entity.HasIndex(e => e.IsActive).HasDatabaseName("IX_Devices_IsActive");
        });

        // WithdrawalRequest entity configuration
        modelBuilder.Entity<WithdrawalRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Amount).HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.BankAccountNumber).HasMaxLength(100);
            entity.Property(e => e.BankAccountName).HasMaxLength(100);
            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.RequestStatus).HasMaxLength(50);
            entity.Property(e => e.RequestedAt);
            entity.Property(e => e.ProcessedAt);
            entity.Property(e => e.RejectionReason).HasMaxLength(500);
            
            entity.HasOne(e => e.Wallet)
                .WithMany(w => w.WithdrawalRequests)
                .HasForeignKey(e => e.WalletId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Configure decimal precision for all decimal properties
        modelBuilder.Entity<Payment>()
            .Property(e => e.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Transaction>()
            .Property(e => e.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Wallet>()
            .Property(e => e.Balance)
            .HasPrecision(18, 2);

        modelBuilder.Entity<PremiumPackage>()
            .Property(e => e.Price)
            .HasPrecision(18, 2);
        // Configure indexes for better performance
        modelBuilder.Entity<Booking>()
            .HasIndex(e => new { e.UserId, e.Status });

        modelBuilder.Entity<Booking>()
            .HasIndex(e => new { e.PhotographerId, e.Status });

        modelBuilder.Entity<Transaction>()
            .HasIndex(e => new { e.FromUserId, e.CreatedAt });

        modelBuilder.Entity<Transaction>()
            .HasIndex(e => new { e.ToUserId, e.CreatedAt });

        modelBuilder.Entity<Rating>()
            .HasIndex(e => new { e.PhotographerId, e.CreatedAt });

        modelBuilder.Entity<Rating>()
            .HasIndex(e => new { e.LocationId, e.CreatedAt });

        // Configure cascade delete behaviors
        modelBuilder.Entity<User>()
            .HasMany(u => u.Photographers)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.LocationOwners)
            .WithOne(lo => lo.User)
            .HasForeignKey(lo => lo.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Photographer>()
            .HasMany(p => p.Bookings)
            .WithOne(b => b.Photographer)
            .HasForeignKey(b => b.PhotographerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Location>()
            .HasMany(l => l.Bookings)
            .WithOne(b => b.Location)
            .HasForeignKey(b => b.LocationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure unique constraints
        modelBuilder.Entity<UserRole>()
            .HasIndex(e => new { e.UserId, e.RoleId })
            .IsUnique();

        modelBuilder.Entity<UserStyle>()
            .HasIndex(e => new { e.UserId, e.StyleId })
            .IsUnique();

        modelBuilder.Entity<PhotographerStyle>()
            .HasIndex(e => new { e.PhotographerId, e.StyleId })
            .IsUnique();

        modelBuilder.Entity<ConversationParticipant>()
            .HasIndex(e => new { e.ConversationId, e.UserId })
            .IsUnique();
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
