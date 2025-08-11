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
        modelBuilder.Entity<Administrator>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Administ__AD0500A64814467C");

            entity.ToTable("Administrator");

            entity.Property(e => e.AdminId).HasColumnName("adminId");
            entity.Property(e => e.AccessLevel)
                .HasMaxLength(30)
                .HasColumnName("accessLevel");
            entity.Property(e => e.Department)
                .HasMaxLength(100)
                .HasColumnName("department");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.Administrators)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Administr__userI__6EF57B66");
        });

        modelBuilder.Entity<Advertisement>(entity =>
        {
            entity.HasKey(e => e.AdvertisementId).HasName("PK__Advertis__4729E935E7D591B6");

            entity.ToTable("Advertisement");

            entity.Property(e => e.AdvertisementId).HasColumnName("advertisementId");
            entity.Property(e => e.Cost)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("cost");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("endDate");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("imageUrl");
            entity.Property(e => e.LocationId).HasColumnName("locationId");
            entity.Property(e => e.PaymentId).HasColumnName("paymentId");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("startDate");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Location).WithMany(p => p.Advertisements)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Advertisement_Location");

            entity.HasOne(d => d.Payment).WithMany(p => p.Advertisements)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK_Advertisement_Payment");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__C6D03BCDDFA35959");

            entity.ToTable("Booking");

            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.EndDatetime)
                .HasColumnType("datetime")
                .HasColumnName("endDatetime");
            entity.Property(e => e.LocationId).HasColumnName("locationId");
            entity.Property(e => e.PhotographerId).HasColumnName("photographerId");
            entity.Property(e => e.SpecialRequests).HasColumnName("specialRequests");
            entity.Property(e => e.StartDatetime)
                .HasColumnType("datetime")
                .HasColumnName("startDatetime");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("totalPrice");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Photographer).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.PhotographerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_Photographer");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_Users");

            entity.HasOne(d => d.Location).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_Location");
        });

        modelBuilder.Entity<Complaint>(entity =>
        {
            entity.HasKey(e => e.ComplaintId).HasName("PK__Complain__489708C12E34784F");

            entity.ToTable("Complaint");

            entity.Property(e => e.ComplaintId).HasColumnName("complaintId");
            entity.Property(e => e.AssignedModeratorId).HasColumnName("assignedModeratorId");
            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.ComplaintType)
                .HasMaxLength(30)
                .HasColumnName("complaintType");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ReportedUserId).HasColumnName("reportedUserId");
            entity.Property(e => e.ReporterId).HasColumnName("reporterId");
            entity.Property(e => e.ResolutionNotes).HasColumnName("resolutionNotes");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.AssignedModerator).WithMany(p => p.Complaints)
                .HasForeignKey(d => d.AssignedModeratorId)
                .HasConstraintName("FK_Complaint_Moderator");

            entity.HasOne(d => d.Booking).WithMany(p => p.Complaints)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_Complaint_Booking");

            entity.HasOne(d => d.ReportedUser).WithMany(p => p.ComplaintReportedUsers)
                .HasForeignKey(d => d.ReportedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Complaint_ReportedUser");

            entity.HasOne(d => d.Reporter).WithMany(p => p.ComplaintReporters)
                .HasForeignKey(d => d.ReporterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Complaint_Reporter");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__Location__30646B6EDF76987D");

            entity.ToTable("Location");

            entity.Property(e => e.LocationId).HasColumnName("locationId");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Amenities).HasColumnName("amenities");
            entity.Property(e => e.AvailabilityStatus)
                .HasMaxLength(30)
                .HasColumnName("availabilityStatus");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FeaturedStatus)
                .HasDefaultValue(false)
                .HasColumnName("featuredStatus");
            entity.Property(e => e.HourlyRate)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("hourlyRate");
            entity.Property(e => e.Indoor)
                .HasDefaultValue(false)
                .HasColumnName("indoor");
            entity.Property(e => e.LocationOwnerId).HasColumnName("locationOwnerId");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Outdoor)
                .HasDefaultValue(false)
                .HasColumnName("outdoor");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.VerificationStatus)
                .HasMaxLength(30)
                .HasColumnName("verificationStatus");

            entity.HasOne(d => d.LocationOwner).WithMany(p => p.Locations)
                .HasForeignKey(d => d.LocationOwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Location_LocationOwner");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Image_Id");
            entity.ToTable("Image");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Url).HasMaxLength(255).HasColumnName("url");
            entity.Property(e => e.IsPrimary).HasDefaultValue(false).HasColumnName("is_primary");
            entity.Property(e => e.Caption).HasColumnName("caption");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.EventId).HasColumnName("event_id");

            entity.HasOne(e => e.User)
                .WithMany(u => u.Images)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Photographer)
                .WithMany(p => p.Images)
                .HasForeignKey(e => e.PhotographerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Location)
                .WithMany(l => l.Images)
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.LocationEvent)
                .WithMany(le => le.Images)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Restrict);

        });

        modelBuilder.Entity<LocationOwner>(entity =>
        {
            entity.HasKey(e => e.LocationOwnerId).HasName("PK__Location__0285A3BB6376E1CF");

            entity.ToTable("LocationOwner");

            entity.Property(e => e.LocationOwnerId).HasColumnName("locationOwnerId");
            entity.Property(e => e.BusinessAddress).HasColumnName("businessAddress");
            entity.Property(e => e.BusinessName)
                .HasMaxLength(255)
                .HasColumnName("businessName");
            entity.Property(e => e.BusinessRegistrationNumber)
                .HasMaxLength(100)
                .HasColumnName("businessRegistrationNumber");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.VerificationStatus)
                .HasMaxLength(30)
                .HasColumnName("verificationStatus");

            entity.HasOne(d => d.User).WithMany(p => p.LocationOwners)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LocationOwner_Users");
        });

        modelBuilder.Entity<Messagess>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__4808B993F0C0786B");

            entity.ToTable("Messagess");

            entity.Property(e => e.MessageId).HasColumnName("messageId");
            entity.Property(e => e.SenderId).HasColumnName("senderId");
            entity.Property(e => e.RecipientId).HasColumnName("recipientId");
            entity.Property(e => e.ConversationId).HasColumnName("conversationId");
            entity.Property(e => e.Content)
                .HasMaxLength(400)
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.MessageType)
                .HasMaxLength(20)
                .HasColumnName("messageType");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.ReadAt)
                .HasColumnType("datetime")
                .HasColumnName("readAt");

            entity.HasOne(d => d.Recipient).WithMany(p => p.MessagessRecipients)
                .HasForeignKey(d => d.RecipientId)
                .HasConstraintName("FK_Messagess_Recipient");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessagessSenders)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK_Messagess_Sender");

            entity.HasOne(d => d.Conversation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ConversationId)
                .HasConstraintName("FK_Messagess_Conversation");

            // Add indexes for better performance
            entity.HasIndex(e => e.SenderId).HasDatabaseName("IX_Messagess_SenderId");
            entity.HasIndex(e => e.RecipientId).HasDatabaseName("IX_Messagess_RecipientId");
            entity.HasIndex(e => e.ConversationId).HasDatabaseName("IX_Messagess_ConversationId");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_Messagess_CreatedAt");
            entity.HasIndex(e => e.Status).HasDatabaseName("IX_Messagess_Status");
        });

        modelBuilder.Entity<Moderator>(entity =>
        {
            entity.HasKey(e => e.ModeratorId).HasName("PK__Moderato__CA327EF233C4DF91");

            entity.ToTable("Moderator");

            entity.Property(e => e.ModeratorId).HasColumnName("moderatorId");
            entity.Property(e => e.AreasOfFocus).HasColumnName("areasOfFocus");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.Moderators)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Moderator_Users");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.MotificationId).HasName("PK__Notifica__9F9C8614B3C7E2AF");

            entity.Property(e => e.MotificationId).HasColumnName("motificationId");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.NotificationType)
                .HasMaxLength(30)
                .HasColumnName("notificationType");
            entity.Property(e => e.ReadStatus)
                .HasDefaultValue(false)
                .HasColumnName("readStatus");
            entity.Property(e => e.ReferenceId).HasColumnName("referenceId");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notifications_Users");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__A0D9EFC6D01669A5");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId).HasColumnName("paymentId");
            entity.Property(e => e.CustomerId).HasColumnName("customerId");
            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("totalAmount");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .HasDefaultValue("VND")
                .HasColumnName("currency");
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.ExternalTransactionId)
                .HasMaxLength(255)
                .HasColumnName("externalTransactionId");
            entity.Property(e => e.Method)
                .HasMaxLength(50)
                .HasDefaultValue("PayOS")
                .HasColumnName("method");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Customer).WithMany()
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_Customer");

            entity.HasOne(d => d.Booking).WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_Booking");
        });

        modelBuilder.Entity<Photographer>(entity =>
        {
            entity.HasKey(e => e.PhotographerId).HasName("PK__Photogra__476AAC030CF022A8");

            entity.ToTable("Photographer");

            entity.Property(e => e.PhotographerId).HasColumnName("photographerId");
            entity.Property(e => e.AvailabilityStatus)
                .HasMaxLength(30)
                .HasColumnName("availabilityStatus");
            entity.Property(e => e.Equipment).HasColumnName("equipment");
            entity.Property(e => e.FeaturedStatus)
                .HasDefaultValue(false)
                .HasColumnName("featuredStatus");
            entity.Property(e => e.HourlyRate)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("hourlyRate");
            entity.Property(e => e.Rating)
                .HasColumnType("decimal(3, 2)")
                .HasColumnName("rating");
            entity.Property(e => e.RatingSum)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("ratingSum");
            entity.Property(e => e.RatingCount)
                .HasColumnName("ratingCount");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.VerificationStatus)
                .HasMaxLength(30)
                .HasColumnName("verificationStatus");
            entity.Property(e => e.YearsExperience).HasColumnName("yearsExperience");

            entity.HasOne(d => d.User).WithMany(p => p.Photographers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Photographer_Users");
        });

        modelBuilder.Entity<PhotographerStyle>(entity =>
        {
            entity.HasKey(e => e.PhotographerStyleId).HasName("PK__Photogra__D64320D6694031E8");

            entity.ToTable("PhotographerStyle");

            entity.Property(e => e.PhotographerStyleId).HasColumnName("photographerStyleId");
            entity.Property(e => e.PhotographerId).HasColumnName("photographerId");
            entity.Property(e => e.StyleId).HasColumnName("styleId");

            entity.HasOne(d => d.Photographer).WithMany(p => p.PhotographerStyles)
                .HasForeignKey(d => d.PhotographerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhotographerStyle_Photographer");

            entity.HasOne(d => d.Style).WithMany(p => p.PhotographerStyles)
                .HasForeignKey(d => d.StyleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhotographerStyle_Style");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.WalletId).HasName("PK__Wallet__A5C5D5C5D5C5D5C5");

            entity.ToTable("Wallet");

            entity.Property(e => e.WalletId).HasColumnName("walletId");
            entity.Property(e => e.Balance)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("balance");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.User).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wallet_User");
        });

        modelBuilder.Entity<PremiumPackage>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("PK__PremiumP__AA8D20C86F773089");

            entity.ToTable("PremiumPackage");

            entity.Property(e => e.PackageId).HasColumnName("packageId");
            entity.Property(e => e.ApplicableTo)
                .HasMaxLength(30)
                .HasColumnName("applicableTo");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DurationDays).HasColumnName("durationDays");
            entity.Property(e => e.Features).HasColumnName("features");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
        });

        modelBuilder.Entity<PremiumSubscription>(entity =>
        {
            entity.HasKey(e => e.PremiumSubscriptionId).HasName("PK__PremiumS__CE9A698AE4183E7A");

            entity.ToTable("PremiumSubscription");

            entity.Property(e => e.PremiumSubscriptionId).HasColumnName("premiumSubscriptionId");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("endDate");
            entity.Property(e => e.PackageId).HasColumnName("packageId");
            entity.Property(e => e.PaymentId).HasColumnName("paymentId");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("startDate");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.PhotographerId).HasColumnName("photographerId");
            entity.Property(e => e.LocationId).HasColumnName("locationId");

            entity.HasOne(d => d.Photographer)
    .WithMany(p => p.PremiumSubscriptions)
    .HasForeignKey(d => d.PhotographerId)
    .OnDelete(DeleteBehavior.NoAction)
    .HasConstraintName("FK_PremiumSubscription_Photographer");

            entity.HasOne(d => d.Location)
    .WithMany(l => l.PremiumSubscriptions)
    .HasForeignKey(d => d.LocationId)
    .OnDelete(DeleteBehavior.NoAction)
    .HasConstraintName("FK_PremiumSubscription_Location");

            // Chỉ cho phép đúng 1 mục tiêu (Photographer XOR Location)
            entity.HasCheckConstraint(
     "CK_PremiumSubscription_ExactlyOneTarget",
     "((photographerId IS NOT NULL AND locationId IS NULL) OR (photographerId IS NULL AND locationId IS NOT NULL))"
 );

            // Mỗi target chỉ có 1 subscription 'active' còn hạn
            entity.HasIndex(e => new { e.PhotographerId, e.Status, e.EndDate })
                .IsUnique()
                .HasFilter("[photographerId] IS NOT NULL AND [Status] = 'active' AND [EndDate] >= getdate()")
                .HasDatabaseName("UX_Sub_Active_Photographer");

            entity.HasIndex(e => new { e.LocationId, e.Status, e.EndDate })
                .IsUnique()
                .HasFilter("[locationId] IS NOT NULL AND [Status] = 'active' AND [EndDate] >= getdate()")
                .HasDatabaseName("UX_Sub_Active_Location");

            entity.HasOne(d => d.Package).WithMany(p => p.PremiumSubscriptions)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PremiumSubscription_PremiumPackage");

            entity.HasOne(d => d.Payment).WithMany(p => p.PremiumSubscriptions)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK_PremiumSubscription_Payment");

            entity.HasOne(d => d.User).WithMany(p => p.PremiumSubscriptions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PremiumSubscription_Users");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Review__2ECD6E04FCF9D780");

            entity.ToTable("Review");

            entity.Property(e => e.ReviewId).HasColumnName("reviewId");
            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.RevieweeId).HasColumnName("revieweeId");
            entity.Property(e => e.RevieweeType)
                .HasMaxLength(30)
                .HasColumnName("revieweeType");
            entity.Property(e => e.ReviewerId).HasColumnName("reviewerId");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Booking).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Review_Booking");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__CD98462AC9EEEE46");

            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.RoleDescription)
                .HasMaxLength(500)
                .HasColumnName("roleDescription");
            entity.Property(e => e.RoleName)
                .HasMaxLength(30)
                .HasColumnName("roleName");
        });

        modelBuilder.Entity<Style>(entity =>
        {
            entity.HasKey(e => e.StyleId).HasName("PK__Style__1F798D1E10021AA4");

            entity.ToTable("Style");

            entity.Property(e => e.StyleId).HasColumnName("styleId");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__9B57CF7272086B9B");

            entity.ToTable("Transaction");

            entity.Property(e => e.TransactionId).HasColumnName("transactionId");
            entity.Property(e => e.ReferencePaymentId).HasColumnName("referencePaymentId");
            entity.Property(e => e.FromUserId).HasColumnName("fromUserId");
            entity.Property(e => e.ToUserId).HasColumnName("toUserId");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .HasDefaultValue("VND")
                .HasColumnName("currency");
            entity.Property(e => e.Type)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasColumnName("type");
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.ReferencePayment).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.ReferencePaymentId)
                .HasConstraintName("FK_Transaction_Payment");

            entity.HasOne(d => d.FromUser).WithMany(p => p.FromTransactions)
                .HasForeignKey(d => d.FromUserId)
                .HasConstraintName("FK_Transaction_FromUser");

            entity.HasOne(d => d.ToUser).WithMany(p => p.ToTransactions)
                .HasForeignKey(d => d.ToUserId)
                .HasConstraintName("FK_Transaction_ToUser");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__CB9A1CFF1A716FE7");

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.Bio)
                .HasMaxLength(300)
                .HasColumnName("bio");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createAt");
            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(50)
                .HasColumnName("fullName");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(100)
                .HasColumnName("passwordHash");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("phoneNumber");
            entity.Property(e => e.ProfileImage)
                .HasMaxLength(200)
                .HasColumnName("profileImage");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updateAt");
            entity.Property(e => e.UserName)
                .HasMaxLength(30)
                .HasColumnName("userName");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__UserRole__CD3149CC42F7DFB1");

            entity.ToTable("UserRole");

            entity.Property(e => e.UserRoleId).HasColumnName("userRoleId");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("assignedAt");
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_UserRole_Role");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserRole_User");
        });

        modelBuilder.Entity<UserStyle>(entity =>
        {
            entity.HasKey(e => e.UserStyleId).HasName("PK__UserStyle__476AAC030CF022A8");

            entity.ToTable("UserStyle");

            entity.Property(e => e.UserStyleId).HasColumnName("userStyleId");
            entity.Property(e => e.StyleId).HasColumnName("styleId");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Style).WithMany(p => p.UserStyles)
                .HasForeignKey(d => d.StyleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserStyle_Style");

            entity.HasOne(d => d.User).WithMany(p => p.UserStyles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserStyle_User");
        });

        modelBuilder.Entity<WithdrawalRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Withdraw__3213E83FE86C5098");

            entity.ToTable("WithdrawalRequest");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.WalletId).HasColumnName("walletId");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.BankAccountName)
                .HasMaxLength(100)
                .HasColumnName("bankAccountName");
            entity.Property(e => e.BankAccountNumber)
                .HasMaxLength(100)
                .HasColumnName("bankAccountNumber");
            entity.Property(e => e.BankName)
                .HasMaxLength(100)
                .HasColumnName("bankName");
            entity.Property(e => e.ProcessedAt)
                .HasColumnType("datetime")
                .HasColumnName("processedAt");
            entity.Property(e => e.ProcessedByModeratorId).HasColumnName("processedByModeratorId");
            entity.Property(e => e.RejectionReason).HasColumnName("rejectionReason");
            entity.Property(e => e.RequestStatus)
                .HasMaxLength(30)
                .HasColumnName("requestStatus");
            entity.Property(e => e.RequestedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("requestedAt");

            entity.HasOne(d => d.Wallet).WithMany(p => p.WithdrawalRequests)
                .HasForeignKey(d => d.WalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WithdrawalRequest_Wallet");
        });


        modelBuilder.Entity<Availability>(entity =>
        {
            entity.HasKey(e => e.AvailabilityId).HasName("PK__Availability__AvailabilityId");

            entity.ToTable("Availability");

            entity.Property(e => e.AvailabilityId).HasColumnName("availabilityId");
            entity.Property(e => e.PhotographerId).HasColumnName("photographerId");
            entity.Property(e => e.DayOfWeek).HasColumnName("dayOfWeek");
            entity.Property(e => e.StartTime).HasColumnName("startTime");
            entity.Property(e => e.EndTime).HasColumnName("endTime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Photographer).WithMany(p => p.Availabilities)
                .HasForeignKey(d => d.PhotographerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Availability_Photographer");

            // Add indexes for better performance
            entity.HasIndex(e => e.PhotographerId).HasDatabaseName("IX_Availability_PhotographerId");
            entity.HasIndex(e => e.DayOfWeek).HasDatabaseName("IX_Availability_DayOfWeek");
            entity.HasIndex(e => e.Status).HasDatabaseName("IX_Availability_Status");

            // Composite indexes for complex queries
            entity.HasIndex(e => new { e.PhotographerId, e.DayOfWeek, e.Status })
                .HasDatabaseName("IX_Availability_Photographer_Day_Status");

            entity.HasIndex(e => new { e.DayOfWeek, e.Status, e.StartTime, e.EndTime })
                .HasDatabaseName("IX_Availability_Day_Status_Time");
        });

        modelBuilder.Entity<PhotoDelivery>(entity =>
        {
            entity.HasKey(e => e.PhotoDeliveryId).HasName("PK__PhotoDelivery__PhotoDeliveryId");

            entity.ToTable("PhotoDelivery");

            entity.Property(e => e.PhotoDeliveryId).HasColumnName("photoDeliveryId");
            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.DeliveryMethod)
                .HasMaxLength(50)
                .HasColumnName("deliveryMethod");
            entity.Property(e => e.DriveLink)
                .HasMaxLength(500)
                .HasColumnName("driveLink");
            entity.Property(e => e.DriveFolderName)
                .HasMaxLength(100)
                .HasColumnName("driveFolderName");
            entity.Property(e => e.PhotoCount).HasColumnName("photoCount");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.UploadedAt)
                .HasColumnType("datetime")
                .HasColumnName("uploadedAt");
            entity.Property(e => e.DeliveredAt)
                .HasColumnType("datetime")
                .HasColumnName("deliveredAt");
            entity.Property(e => e.ExpiresAt)
                .HasColumnType("datetime")
                .HasColumnName("expiresAt");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Booking).WithOne(p => p.PhotoDelivery)
                .HasForeignKey<PhotoDelivery>(d => d.BookingId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PhotoDelivery_Booking");

            // Add indexes for better performance
            entity.HasIndex(e => e.BookingId).HasDatabaseName("IX_PhotoDelivery_BookingId");
            entity.HasIndex(e => e.Status).HasDatabaseName("IX_PhotoDelivery_Status");
            entity.HasIndex(e => e.DeliveryMethod).HasDatabaseName("IX_PhotoDelivery_DeliveryMethod");
        });

        modelBuilder.Entity<DeviceInfo>(entity =>
        {
            entity.HasKey(e => e.DeviceInfoId).HasName("PK__DeviceInfo__DeviceInfoId");

            entity.ToTable("DeviceInfo");

            entity.Property(e => e.DeviceInfoId).HasColumnName("deviceInfoId");
            entity.Property(e => e.PhotographerId).HasColumnName("photographerId");
            entity.Property(e => e.DeviceType)
                .HasMaxLength(100)
                .HasColumnName("deviceType");
            entity.Property(e => e.Brand)
                .HasMaxLength(100)
                .HasColumnName("brand");
            entity.Property(e => e.Model)
                .HasMaxLength(100)
                .HasColumnName("model");
            entity.Property(e => e.OperatingSystem)
                .HasMaxLength(20)
                .HasColumnName("operatingSystem");
            entity.Property(e => e.OsVersion)
                .HasMaxLength(20)
                .HasColumnName("osVersion");
            entity.Property(e => e.ScreenResolution)
                .HasMaxLength(50)
                .HasColumnName("screenResolution");
            entity.Property(e => e.CameraResolution)
                .HasMaxLength(20)
                .HasColumnName("cameraResolution");
            entity.Property(e => e.StorageCapacity)
                .HasMaxLength(100)
                .HasColumnName("storageCapacity");
            entity.Property(e => e.BatteryCapacity)
                .HasMaxLength(100)
                .HasColumnName("batteryCapacity");
            entity.Property(e => e.Features)
                .HasMaxLength(500)
                .HasColumnName("features");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.LastUsedAt)
                .HasColumnType("datetime")
                .HasColumnName("lastUsedAt");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Photographer).WithMany(p => p.DeviceInfos)
                .HasForeignKey(d => d.PhotographerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_DeviceInfo_Photographer");

            // Add indexes for better performance
            entity.HasIndex(e => e.PhotographerId).HasDatabaseName("IX_DeviceInfo_PhotographerId");
            entity.HasIndex(e => e.DeviceType).HasDatabaseName("IX_DeviceInfo_DeviceType");
            entity.HasIndex(e => e.Brand).HasDatabaseName("IX_DeviceInfo_Brand");
            entity.HasIndex(e => e.Status).HasDatabaseName("IX_DeviceInfo_Status");
            entity.HasIndex(e => e.LastUsedAt).HasDatabaseName("IX_DeviceInfo_LastUsedAt");
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.ConversationId).HasName("PK__Conversation__ConversationId");

            entity.ToTable("Conversation");

            entity.Property(e => e.ConversationId).HasColumnName("conversationId");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");

            // Add indexes for better performance
            entity.HasIndex(e => e.Status).HasDatabaseName("IX_Conversation_Status");
            entity.HasIndex(e => e.Type).HasDatabaseName("IX_Conversation_Type");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_Conversation_CreatedAt");
        });

        modelBuilder.Entity<ConversationParticipant>(entity =>
        {
            entity.HasKey(e => e.ConversationParticipantId).HasName("PK__ConversationParticipant__ConversationParticipantId");

            entity.ToTable("ConversationParticipant");

            entity.Property(e => e.ConversationParticipantId).HasColumnName("conversationParticipantId");
            entity.Property(e => e.ConversationId).HasColumnName("conversationId");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("joinedAt");
            entity.Property(e => e.LeftAt)
                .HasColumnType("datetime")
                .HasColumnName("leftAt");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");
            entity.Property(e => e.IsActive)
                .HasColumnName("isActive");

            entity.HasOne(d => d.Conversation).WithMany(p => p.Participants)
                .HasForeignKey(d => d.ConversationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ConversationParticipant_Conversation");

            entity.HasOne(d => d.User).WithMany(p => p.ConversationParticipants)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ConversationParticipant_User");

            // Add indexes for better performance
            entity.HasIndex(e => e.ConversationId).HasDatabaseName("IX_ConversationParticipant_ConversationId");
            entity.HasIndex(e => e.UserId).HasDatabaseName("IX_ConversationParticipant_UserId");
            entity.HasIndex(e => e.IsActive).HasDatabaseName("IX_ConversationParticipant_IsActive");
            entity.HasIndex(e => e.Role).HasDatabaseName("IX_ConversationParticipant_Role");
        });

        modelBuilder.Entity<LocationEvent>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__LocationEvent__EventId");

            entity.ToTable("LocationEvent");

            entity.Property(e => e.EventId).HasColumnName("eventId");
            entity.Property(e => e.LocationId).HasColumnName("locationId");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("startDate");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("endDate");
            entity.Property(e => e.DiscountedPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("discountedPrice");
            entity.Property(e => e.OriginalPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("originalPrice");
            entity.Property(e => e.MaxPhotographers)
                .HasColumnName("maxPhotographers");
            entity.Property(e => e.MaxBookingsPerSlot)
                .HasColumnName("maxBookingsPerSlot");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Location).WithMany(p => p.LocationEvents)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_LocationEvent_Location");

            // Add indexes for better performance
            entity.HasIndex(e => e.LocationId).HasDatabaseName("IX_LocationEvent_LocationId");
            entity.HasIndex(e => e.Status).HasDatabaseName("IX_LocationEvent_Status");
            entity.HasIndex(e => e.StartDate).HasDatabaseName("IX_LocationEvent_StartDate");
            entity.HasIndex(e => e.EndDate).HasDatabaseName("IX_LocationEvent_EndDate");
            entity.HasIndex(e => new { e.Status, e.StartDate, e.EndDate })
                .HasDatabaseName("IX_LocationEvent_Status_DateRange");
        });

        modelBuilder.Entity<EventPhotographer>(entity =>
        {
            entity.HasKey(e => e.EventPhotographerId).HasName("PK__EventPhotographer__EventPhotographerId");

            entity.ToTable("EventPhotographer");

            entity.Property(e => e.EventPhotographerId).HasColumnName("eventPhotographerId");
            entity.Property(e => e.EventId).HasColumnName("eventId");
            entity.Property(e => e.PhotographerId).HasColumnName("photographerId");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.AppliedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("appliedAt");
            entity.Property(e => e.ApprovedAt)
                .HasColumnType("datetime")
                .HasColumnName("approvedAt");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500)
                .HasColumnName("rejectionReason");
            entity.Property(e => e.SpecialRate)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("specialRate");

            entity.HasOne(d => d.Event).WithMany(p => p.EventPhotographers)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_EventPhotographer_LocationEvent");

            entity.HasOne(d => d.Photographer).WithMany(p => p.EventPhotographers)
                .HasForeignKey(d => d.PhotographerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_EventPhotographer_Photographer");

            // Add indexes for better performance
            entity.HasIndex(e => e.EventId).HasDatabaseName("IX_EventPhotographer_EventId");
            entity.HasIndex(e => e.PhotographerId).HasDatabaseName("IX_EventPhotographer_PhotographerId");
            entity.HasIndex(e => e.Status).HasDatabaseName("IX_EventPhotographer_Status");
            entity.HasIndex(e => new { e.EventId, e.PhotographerId })
                .IsUnique()
                .HasDatabaseName("IX_EventPhotographer_Event_Photographer_Unique");
        });

        modelBuilder.Entity<EventBooking>(entity =>
        {
            entity.HasKey(e => e.EventBookingId).HasName("PK__EventBooking__EventBookingId");

            entity.ToTable("EventBooking");

            entity.Property(e => e.EventBookingId).HasColumnName("eventBookingId");
            entity.Property(e => e.EventId).HasColumnName("eventId");
            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.EventPhotographerId).HasColumnName("eventPhotographerId");
            entity.Property(e => e.EventPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("eventPrice");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");

            entity.HasOne(d => d.Event).WithMany(p => p.EventBookings)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_EventBooking_LocationEvent");

            entity.HasOne(d => d.Booking).WithOne(p => p.EventBooking)
                .HasForeignKey<EventBooking>(d => d.BookingId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_EventBooking_Booking");

            entity.HasOne(d => d.EventPhotographer).WithMany()
                .HasForeignKey(d => d.EventPhotographerId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_EventBooking_EventPhotographer");

            // Add indexes for better performance
            entity.HasIndex(e => e.EventId).HasDatabaseName("IX_EventBooking_EventId");
            entity.HasIndex(e => e.BookingId).HasDatabaseName("IX_EventBooking_BookingId");
            entity.HasIndex(e => e.EventPhotographerId).HasDatabaseName("IX_EventBooking_EventPhotographerId");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_EventBooking_CreatedAt");
        });
        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__Rating__ratingId");

            entity.ToTable("Rating");

            // Column mapping
            entity.Property(e => e.RatingId).HasColumnName("ratingId");
            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.ReviewerUserId).HasColumnName("reviewerUserId");
            entity.Property(e => e.PhotographerId).HasColumnName("photographerId");
            entity.Property(e => e.LocationId).HasColumnName("locationId");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("(getdate())")
                  .HasColumnType("datetime")
                  .HasColumnName("createdAt");
            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("(getdate())")
                  .HasColumnType("datetime")
                  .HasColumnName("updatedAt");

            // Check constraints
            entity.HasCheckConstraint("CK_Rating_Score_1_5", "[score] BETWEEN 1 AND 5");
            entity.HasCheckConstraint(
                "CK_Rating_ExactlyOneTarget",
                "((photographerId IS NOT NULL AND locationId IS NULL) OR (photographerId IS NULL AND locationId IS NOT NULL))"
            );

            // Relationships
            entity.HasOne(e => e.Booking)
                .WithMany(b => b.Ratings)
                .HasForeignKey(e => e.BookingId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Rating_Booking");

            entity.HasOne(e => e.ReviewerUser)
                .WithMany(u => u.RatingsAuthored)
                .HasForeignKey(e => e.ReviewerUserId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Rating_ReviewerUser");

            entity.HasOne(e => e.Photographer)
                .WithMany(p => p.Ratings)
                .HasForeignKey(e => e.PhotographerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Rating_Photographer");

            entity.HasOne(e => e.Location)
                .WithMany(l => l.Ratings)
                .HasForeignKey(e => e.LocationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Rating_Location");

            // Unique per booking per target (filtered unique index – SQL Server)
            entity.HasIndex(e => new { e.BookingId, e.PhotographerId })
                .IsUnique()
                .HasFilter("[photographerId] IS NOT NULL")
                .HasDatabaseName("UX_Rating_Booking_Photographer");

            entity.HasIndex(e => new { e.BookingId, e.LocationId })
                .IsUnique()
                .HasFilter("[locationId] IS NOT NULL")
                .HasDatabaseName("UX_Rating_Booking_Location");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
