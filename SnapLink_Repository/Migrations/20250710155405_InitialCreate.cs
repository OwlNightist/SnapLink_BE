using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnapLink_Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ref_id = table.Column<int>(type: "int", nullable: false),
                    is_primary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Caption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image_Id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PremiumPackage",
                columns: table => new
                {
                    packageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    durationDays = table.Column<int>(type: "int", nullable: true),
                    features = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    applicableTo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PremiumP__AA8D20C86F773089", x => x.packageId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    roleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    roleName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    roleDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__CD98462AC9EEEE46", x => x.roleId);
                });

            migrationBuilder.CreateTable(
                name: "Style",
                columns: table => new
                {
                    styleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Style__1F798D1E10021AA4", x => x.styleId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    userId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    email = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    passwordHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    phoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    fullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    profileImage = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    bio = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    createAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updateAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__CB9A1CFF1A716FE7", x => x.userId);
                });

            migrationBuilder.CreateTable(
                name: "Administrator",
                columns: table => new
                {
                    adminId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    accessLevel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Administ__AD0500A64814467C", x => x.adminId);
                    table.ForeignKey(
                        name: "FK__Administr__userI__6EF57B66",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "LocationOwner",
                columns: table => new
                {
                    locationOwnerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    businessName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    businessAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    businessRegistrationNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    verificationStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Location__0285A3BB6376E1CF", x => x.locationOwnerId);
                    table.ForeignKey(
                        name: "FK_LocationOwner_Users",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "Messagess",
                columns: table => new
                {
                    messageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    senderId = table.Column<int>(type: "int", nullable: true),
                    recipientId = table.Column<int>(type: "int", nullable: true),
                    content = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    attachmentUrl = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    readStatus = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Messages__4808B993F0C0786B", x => x.messageId);
                    table.ForeignKey(
                        name: "FK_Messagess_Recipient",
                        column: x => x.recipientId,
                        principalTable: "Users",
                        principalColumn: "userId");
                    table.ForeignKey(
                        name: "FK_Messagess_Sender",
                        column: x => x.senderId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "Moderator",
                columns: table => new
                {
                    moderatorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    areasOfFocus = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Moderato__CA327EF233C4DF91", x => x.moderatorId);
                    table.ForeignKey(
                        name: "FK_Moderator_Users",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    motificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    notificationType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    referenceId = table.Column<int>(type: "int", nullable: true),
                    readStatus = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__9F9C8614B3C7E2AF", x => x.motificationId);
                    table.ForeignKey(
                        name: "FK_Notifications_Users",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "Photographer",
                columns: table => new
                {
                    photographerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    yearsExperience = table.Column<int>(type: "int", nullable: true),
                    equipment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    hourlyRate = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    availabilityStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    rating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    ratingSum = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ratingCount = table.Column<int>(type: "int", nullable: true),
                    featuredStatus = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    verificationStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Photogra__476AAC030CF022A8", x => x.photographerId);
                    table.ForeignKey(
                        name: "FK_Photographer_Users",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    transactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Transact__9B57CF7272086B9B", x => x.transactionId);
                    table.ForeignKey(
                        name: "FK_Transactions_Users",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    userRoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: true),
                    roleId = table.Column<int>(type: "int", nullable: true),
                    assignedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserRole__CD3149CC42F7DFB1", x => x.userRoleId);
                    table.ForeignKey(
                        name: "FK_UserRole_Role",
                        column: x => x.roleId,
                        principalTable: "Roles",
                        principalColumn: "roleId");
                    table.ForeignKey(
                        name: "FK_UserRole_User",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "UserStyle",
                columns: table => new
                {
                    userStyleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    styleId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserStyle__476AAC030CF022A8", x => x.userStyleId);
                    table.ForeignKey(
                        name: "FK_UserStyle_Style",
                        column: x => x.styleId,
                        principalTable: "Style",
                        principalColumn: "styleId");
                    table.ForeignKey(
                        name: "FK_UserStyle_User",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    locationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    locationOwnerId = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    amenities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    hourlyRate = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    capacity = table.Column<int>(type: "int", nullable: true),
                    indoor = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    outdoor = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    availabilityStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    featuredStatus = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    verificationStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Location__30646B6EDF76987D", x => x.locationId);
                    table.ForeignKey(
                        name: "FK_Location_LocationOwner",
                        column: x => x.locationOwnerId,
                        principalTable: "LocationOwner",
                        principalColumn: "locationOwnerId");
                });

            migrationBuilder.CreateTable(
                name: "PhotographerEvent",
                columns: table => new
                {
                    eventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    photographerId = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    originalPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    discountedPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    discountPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    startDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    endDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    maxBookings = table.Column<int>(type: "int", nullable: true),
                    currentBookings = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PhotographerEvent__EventId", x => x.eventId);
                    table.ForeignKey(
                        name: "FK_PhotographerEvent_Photographer",
                        column: x => x.photographerId,
                        principalTable: "Photographer",
                        principalColumn: "photographerId");
                });

            migrationBuilder.CreateTable(
                name: "PhotographerStyle",
                columns: table => new
                {
                    photographerStyleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    photographerId = table.Column<int>(type: "int", nullable: false),
                    styleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Photogra__D64320D6694031E8", x => x.photographerStyleId);
                    table.ForeignKey(
                        name: "FK_PhotographerStyle_Photographer",
                        column: x => x.photographerId,
                        principalTable: "Photographer",
                        principalColumn: "photographerId");
                    table.ForeignKey(
                        name: "FK_PhotographerStyle_Style",
                        column: x => x.styleId,
                        principalTable: "Style",
                        principalColumn: "styleId");
                });

            migrationBuilder.CreateTable(
                name: "PhotographerWallet",
                columns: table => new
                {
                    photographerWalletId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    photographerId = table.Column<int>(type: "int", nullable: false),
                    balance = table.Column<decimal>(type: "decimal(10,2)", nullable: true, defaultValue: 0m),
                    updatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Photogra__99B37BECD7BD3C88", x => x.photographerWalletId);
                    table.ForeignKey(
                        name: "FK_PhotographerWallet_Photographer",
                        column: x => x.photographerId,
                        principalTable: "Photographer",
                        principalColumn: "photographerId");
                });

            migrationBuilder.CreateTable(
                name: "WithdrawalRequest",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    photographerId = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    bankAccountNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    bankAccountName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    bankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    requestStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    requestedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    processedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    processedByModeratorId = table.Column<int>(type: "int", nullable: true),
                    rejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Withdraw__3213E83FE86C5098", x => x.id);
                    table.ForeignKey(
                        name: "FK_WithdrawalRequest_Photographer",
                        column: x => x.photographerId,
                        principalTable: "Photographer",
                        principalColumn: "photographerId");
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    bookingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    photographerId = table.Column<int>(type: "int", nullable: false),
                    locationId = table.Column<int>(type: "int", nullable: false),
                    eventId = table.Column<int>(type: "int", nullable: true),
                    startDatetime = table.Column<DateTime>(type: "datetime", nullable: true),
                    endDatetime = table.Column<DateTime>(type: "datetime", nullable: true),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    specialRequests = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    totalPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Booking__C6D03BCDDFA35959", x => x.bookingId);
                    table.ForeignKey(
                        name: "FK_Booking_Event",
                        column: x => x.eventId,
                        principalTable: "PhotographerEvent",
                        principalColumn: "eventId");
                    table.ForeignKey(
                        name: "FK_Booking_Photographer",
                        column: x => x.photographerId,
                        principalTable: "Photographer",
                        principalColumn: "photographerId");
                    table.ForeignKey(
                        name: "FK_Booking_Users",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "PhotographerEventLocation",
                columns: table => new
                {
                    eventLocationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    eventId = table.Column<int>(type: "int", nullable: false),
                    locationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PhotographerEventLocation__EventLocationId", x => x.eventLocationId);
                    table.ForeignKey(
                        name: "FK_PhotographerEventLocation_Event",
                        column: x => x.eventId,
                        principalTable: "PhotographerEvent",
                        principalColumn: "eventId");
                    table.ForeignKey(
                        name: "FK_PhotographerEventLocation_Location",
                        column: x => x.locationId,
                        principalTable: "Location",
                        principalColumn: "locationId");
                });

            migrationBuilder.CreateTable(
                name: "Complaint",
                columns: table => new
                {
                    complaintId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    reporterId = table.Column<int>(type: "int", nullable: false),
                    reportedUserId = table.Column<int>(type: "int", nullable: false),
                    bookingId = table.Column<int>(type: "int", nullable: true),
                    complaintType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    assignedModeratorId = table.Column<int>(type: "int", nullable: true),
                    resolutionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Complain__489708C12E34784F", x => x.complaintId);
                    table.ForeignKey(
                        name: "FK_Complaint_Booking",
                        column: x => x.bookingId,
                        principalTable: "Booking",
                        principalColumn: "bookingId");
                    table.ForeignKey(
                        name: "FK_Complaint_Moderator",
                        column: x => x.assignedModeratorId,
                        principalTable: "Moderator",
                        principalColumn: "moderatorId");
                    table.ForeignKey(
                        name: "FK_Complaint_ReportedUser",
                        column: x => x.reportedUserId,
                        principalTable: "Users",
                        principalColumn: "userId");
                    table.ForeignKey(
                        name: "FK_Complaint_Reporter",
                        column: x => x.reporterId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    paymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    bookingId = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    paymentMethod = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    transactionId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    photographerPayoutAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    locationOwnerPayoutAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    platformFee = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payment__A0D9EFC6D01669A5", x => x.paymentId);
                    table.ForeignKey(
                        name: "FK_Payment_Booking_bookingId",
                        column: x => x.bookingId,
                        principalTable: "Booking",
                        principalColumn: "bookingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    reviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    bookingId = table.Column<int>(type: "int", nullable: false),
                    reviewerId = table.Column<int>(type: "int", nullable: false),
                    revieweeType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    revieweeId = table.Column<int>(type: "int", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Review__2ECD6E04FCF9D780", x => x.reviewId);
                    table.ForeignKey(
                        name: "FK_Review_Booking",
                        column: x => x.bookingId,
                        principalTable: "Booking",
                        principalColumn: "bookingId");
                });

            migrationBuilder.CreateTable(
                name: "Advertisement",
                columns: table => new
                {
                    advertisementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    locationId = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    imageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    startDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    endDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    cost = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    paymentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Advertis__4729E935E7D591B6", x => x.advertisementId);
                    table.ForeignKey(
                        name: "FK_Advertisement_Location",
                        column: x => x.locationId,
                        principalTable: "Location",
                        principalColumn: "locationId");
                    table.ForeignKey(
                        name: "FK_Advertisement_Payment",
                        column: x => x.paymentId,
                        principalTable: "Payment",
                        principalColumn: "paymentId");
                });

            migrationBuilder.CreateTable(
                name: "PremiumSubscription",
                columns: table => new
                {
                    premiumSubscriptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    packageId = table.Column<int>(type: "int", nullable: false),
                    startDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    endDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    paymentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PremiumS__CE9A698AE4183E7A", x => x.premiumSubscriptionId);
                    table.ForeignKey(
                        name: "FK_PremiumSubscription_Payment",
                        column: x => x.paymentId,
                        principalTable: "Payment",
                        principalColumn: "paymentId");
                    table.ForeignKey(
                        name: "FK_PremiumSubscription_PremiumPackage",
                        column: x => x.packageId,
                        principalTable: "PremiumPackage",
                        principalColumn: "packageId");
                    table.ForeignKey(
                        name: "FK_PremiumSubscription_Users",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administrator_userId",
                table: "Administrator",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisement_locationId",
                table: "Advertisement",
                column: "locationId");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisement_paymentId",
                table: "Advertisement",
                column: "paymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_eventId",
                table: "Booking",
                column: "eventId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_photographerId",
                table: "Booking",
                column: "photographerId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_userId",
                table: "Booking",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaint_assignedModeratorId",
                table: "Complaint",
                column: "assignedModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaint_bookingId",
                table: "Complaint",
                column: "bookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaint_reportedUserId",
                table: "Complaint",
                column: "reportedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaint_reporterId",
                table: "Complaint",
                column: "reporterId");

            migrationBuilder.CreateIndex(
                name: "IX_Location_locationOwnerId",
                table: "Location",
                column: "locationOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationOwner_userId",
                table: "LocationOwner",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Messagess_recipientId",
                table: "Messagess",
                column: "recipientId");

            migrationBuilder.CreateIndex(
                name: "IX_Messagess_senderId",
                table: "Messagess",
                column: "senderId");

            migrationBuilder.CreateIndex(
                name: "IX_Moderator_userId",
                table: "Moderator",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_userId",
                table: "Notifications",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_bookingId",
                table: "Payment",
                column: "bookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Photographer_userId",
                table: "Photographer",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotographerEvent_photographerId",
                table: "PhotographerEvent",
                column: "photographerId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotographerEventLocation_eventId",
                table: "PhotographerEventLocation",
                column: "eventId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotographerEventLocation_locationId",
                table: "PhotographerEventLocation",
                column: "locationId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotographerStyle_photographerId",
                table: "PhotographerStyle",
                column: "photographerId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotographerStyle_styleId",
                table: "PhotographerStyle",
                column: "styleId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotographerWallet_photographerId",
                table: "PhotographerWallet",
                column: "photographerId");

            migrationBuilder.CreateIndex(
                name: "IX_PremiumSubscription_packageId",
                table: "PremiumSubscription",
                column: "packageId");

            migrationBuilder.CreateIndex(
                name: "IX_PremiumSubscription_paymentId",
                table: "PremiumSubscription",
                column: "paymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PremiumSubscription_userId",
                table: "PremiumSubscription",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_bookingId",
                table: "Review",
                column: "bookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_userId",
                table: "Transactions",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_roleId",
                table: "UserRole",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_userId",
                table: "UserRole",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_UserStyle_styleId",
                table: "UserStyle",
                column: "styleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserStyle_userId",
                table: "UserStyle",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalRequest_photographerId",
                table: "WithdrawalRequest",
                column: "photographerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrator");

            migrationBuilder.DropTable(
                name: "Advertisement");

            migrationBuilder.DropTable(
                name: "Complaint");

            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.DropTable(
                name: "Messagess");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PhotographerEventLocation");

            migrationBuilder.DropTable(
                name: "PhotographerStyle");

            migrationBuilder.DropTable(
                name: "PhotographerWallet");

            migrationBuilder.DropTable(
                name: "PremiumSubscription");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "UserStyle");

            migrationBuilder.DropTable(
                name: "WithdrawalRequest");

            migrationBuilder.DropTable(
                name: "Moderator");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "PremiumPackage");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Style");

            migrationBuilder.DropTable(
                name: "LocationOwner");

            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "PhotographerEvent");

            migrationBuilder.DropTable(
                name: "Photographer");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
