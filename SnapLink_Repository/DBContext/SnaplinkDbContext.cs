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

    public virtual DbSet<Moderator> Moderators { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Photographer> Photographers { get; set; }

    public virtual DbSet<PhotographerStyle> PhotographerStyles { get; set; }

    public virtual DbSet<PhotographerWallet> PhotographerWallets { get; set; }

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

    public virtual DbSet<PhotographerEvent> PhotographerEvents { get; set; }

    public virtual DbSet<PhotographerEventLocation> PhotographerEventLocations { get; set; }

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
            entity.Property(e => e.EventId).HasColumnName("eventId");
            entity.Property(e => e.SpecialRequests).HasColumnName("specialRequests");
            entity.Property(e => e.StartDatetime)
                .HasColumnType("datetime")
                .HasColumnName("startDatetime");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(10, 2)")
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

            entity.HasOne(d => d.Event).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("FK_Booking_Event");
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
            entity.Property(e => e.Type).HasMaxLength(50).HasColumnName("type");
            entity.Property(e => e.RefId).HasColumnName("ref_id");
            entity.Property(e => e.IsPrimary).HasDefaultValue(false).HasColumnName("is_primary");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
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
            entity.Property(e => e.AttachmentUrl)
                .HasMaxLength(100)
                .HasColumnName("attachmentUrl");
            entity.Property(e => e.Content)
                .HasMaxLength(400)
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.ReadStatus)
                .HasDefaultValue(false)
                .HasColumnName("readStatus");
            entity.Property(e => e.RecipientId).HasColumnName("recipientId");
            entity.Property(e => e.SenderId).HasColumnName("senderId");

            entity.HasOne(d => d.Recipient).WithMany(p => p.MessagessRecipients)
                .HasForeignKey(d => d.RecipientId)
                .HasConstraintName("FK_Messagess_Recipient");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessagessSenders)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK_Messagess_Sender");
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
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.LocationOwnerPayoutAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("locationOwnerPayoutAmount");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(30)
                .HasColumnName("paymentMethod");
            entity.Property(e => e.PhotographerPayoutAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("photographerPayoutAmount");
            entity.Property(e => e.PlatformFee)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("platformFee");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(255)
                .HasColumnName("transactionId");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
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

        modelBuilder.Entity<PhotographerWallet>(entity =>
        {
            entity.HasKey(e => e.PhotographerWalletId).HasName("PK__Photogra__99B37BECD7BD3C88");

            entity.ToTable("PhotographerWallet");

            entity.Property(e => e.PhotographerWalletId).HasColumnName("photographerWalletId");
            entity.Property(e => e.Balance)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("balance");
            entity.Property(e => e.PhotographerId).HasColumnName("photographerId");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Photographer).WithMany(p => p.PhotographerWallets)
                .HasForeignKey(d => d.PhotographerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhotographerWallet_Photographer");
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

            entity.Property(e => e.TransactionId).HasColumnName("transactionId");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(30)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transactions_Users");
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
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
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
            entity.Property(e => e.PhotographerId).HasColumnName("photographerId");
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

            entity.HasOne(d => d.Photographer).WithMany(p => p.WithdrawalRequests)
                .HasForeignKey(d => d.PhotographerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WithdrawalRequest_Photographer");
        });

        modelBuilder.Entity<PhotographerEvent>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__PhotographerEvent__EventId");

            entity.ToTable("PhotographerEvent");

            entity.Property(e => e.EventId).HasColumnName("eventId");
            entity.Property(e => e.PhotographerId).HasColumnName("photographerId");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.OriginalPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("originalPrice");
            entity.Property(e => e.DiscountedPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("discountedPrice");
            entity.Property(e => e.DiscountPercentage)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("discountPercentage");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("startDate");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("endDate");
            entity.Property(e => e.MaxBookings).HasColumnName("maxBookings");
            entity.Property(e => e.CurrentBookings).HasColumnName("currentBookings");
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

            entity.HasOne(d => d.Photographer).WithMany(p => p.PhotographerEvents)
                .HasForeignKey(d => d.PhotographerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhotographerEvent_Photographer");
        });

        modelBuilder.Entity<PhotographerEventLocation>(entity =>
        {
            entity.HasKey(e => e.EventLocationId).HasName("PK__PhotographerEventLocation__EventLocationId");

            entity.ToTable("PhotographerEventLocation");

            entity.Property(e => e.EventLocationId).HasColumnName("eventLocationId");
            entity.Property(e => e.EventId).HasColumnName("eventId");
            entity.Property(e => e.LocationId).HasColumnName("locationId");

            entity.HasOne(d => d.Event).WithMany(p => p.PhotographerEventLocations)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhotographerEventLocation_Event");

            entity.HasOne(d => d.Location).WithMany(p => p.PhotographerEventLocations)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhotographerEventLocation_Location");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
