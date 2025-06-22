using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SnapLink_Repository.DBContext;
using SnapLink_Repository.Entity;

namespace SnapLink_Repository.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SnaplinkDbContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if data already exists
            if (context.Users.Any())
            {
                return; // Database has been seeded
            }

            // Seed Roles
            var roles = SeedRoles(context);
            
            // Seed Styles
            var styles = SeedStyles(context);
            
            // Seed Premium Packages
            var packages = SeedPremiumPackages(context);
            
            // Seed Users
            var users = SeedUsers(context);
            
            // Seed User Roles
            SeedUserRoles(context, users, roles);
            
            // Seed Administrators
            SeedAdministrators(context, users);
            
            // Seed Moderators
            SeedModerators(context, users);
            
            // Seed Photographers
            var photographers = SeedPhotographers(context, users);
            
            // Seed Photographer Styles
            SeedPhotographerStyles(context, photographers, styles);
            
            // Seed Photographer Wallets
            SeedPhotographerWallets(context, photographers);
            
            // Seed Location Owners
            var locationOwners = SeedLocationOwners(context, users);
            
            // Seed Locations
            var locations = SeedLocations(context, locationOwners);
            
            // Seed Location Images
            SeedLocationImages(context, locations);
            
            // Seed Advertisements
            SeedAdvertisements(context, locations);
            
            // Seed Bookings
            var bookings = SeedBookings(context, users, photographers, locations);
            
            // Seed Reviews
            SeedReviews(context, bookings);
            
            // Seed Payments
            var payments = SeedPayments(context, bookings);
            
            // Seed Premium Subscriptions
            SeedPremiumSubscriptions(context, users, packages, payments);
            
            // Seed Transactions
            SeedTransactions(context, users);
            
            // Seed Notifications
            SeedNotifications(context, users);
            
            // Seed Messages
            SeedMessages(context, users);
            
            // Seed Complaints
            SeedComplaints(context, users, bookings);
            
            // Seed Withdrawal Requests
            SeedWithdrawalRequests(context, photographers);

            context.SaveChanges();
        }

        private static List<Role> SeedRoles(SnaplinkDbContext context)
        {
            var roles = new List<Role>
            {
                new Role { RoleName = "Admin", RoleDescription = "System Administrator with full access" },
                new Role { RoleName = "User", RoleDescription = "Regular user with basic access" },
                new Role { RoleName = "Photographer", RoleDescription = "Professional photographer" },
                new Role { RoleName = "LocationOwner", RoleDescription = "Owner of photography locations" },
                new Role { RoleName = "Moderator", RoleDescription = "Content moderator" }
            };

            context.Roles.AddRange(roles);
            context.SaveChanges();
            return roles;
        }

        private static List<Style> SeedStyles(SnaplinkDbContext context)
        {
            var styles = new List<Style>
            {
                new Style { Name = "Portrait", Description = "Professional portrait photography" },
                new Style { Name = "Wedding", Description = "Wedding and event photography" },
                new Style { Name = "Landscape", Description = "Nature and landscape photography" },
                new Style { Name = "Street", Description = "Urban and street photography" },
                new Style { Name = "Fashion", Description = "Fashion and editorial photography" },
                new Style { Name = "Product", Description = "Commercial product photography" },
                new Style { Name = "Architecture", Description = "Architectural photography" },
                new Style { Name = "Documentary", Description = "Documentary and photojournalism" }
            };

            context.Styles.AddRange(styles);
            context.SaveChanges();
            return styles;
        }

        private static List<PremiumPackage> SeedPremiumPackages(SnaplinkDbContext context)
        {
            var packages = new List<PremiumPackage>
            {
                new PremiumPackage 
                { 
                    Name = "Basic Premium", 
                    Description = "Basic premium features for photographers",
                    Price = 29.99m,
                    DurationDays = 30,
                    Features = "Priority listing, Basic analytics, Email support",
                    ApplicableTo = "Photographer"
                },
                new PremiumPackage 
                { 
                    Name = "Professional Premium", 
                    Description = "Professional features for serious photographers",
                    Price = 79.99m,
                    DurationDays = 30,
                    Features = "Priority listing, Advanced analytics, Phone support, Featured profile",
                    ApplicableTo = "Photographer"
                },
                new PremiumPackage 
                { 
                    Name = "Location Owner Premium", 
                    Description = "Premium features for location owners",
                    Price = 49.99m,
                    DurationDays = 30,
                    Features = "Priority listing, Analytics, Customer management",
                    ApplicableTo = "LocationOwner"
                }
            };

            context.PremiumPackages.AddRange(packages);
            context.SaveChanges();
            return packages;
        }

        private static List<User> SeedUsers(SnaplinkDbContext context)
        {
            var users = new List<User>
            {
                // Admin users
                new User 
                { 
                    UserName = "admin1", 
                    Email = "admin1@snaplink.com", 
                    PasswordHash = "hashed_password_1",
                    PhoneNumber = "+1234567890",
                    FullName = "John Admin",
                    ProfileImage = "https://example.com/admin1.jpg",
                    Bio = "System Administrator",
                    CreateAt = DateTime.Now.AddDays(-365),
                    UpdateAt = DateTime.Now,
                    Status = "Active"
                },
                
                // Regular users
                new User 
                { 
                    UserName = "user1", 
                    Email = "user1@example.com", 
                    PasswordHash = "hashed_password_2",
                    PhoneNumber = "+1234567891",
                    FullName = "Alice Johnson",
                    ProfileImage = "https://example.com/user1.jpg",
                    Bio = "Photography enthusiast",
                    CreateAt = DateTime.Now.AddDays(-300),
                    UpdateAt = DateTime.Now,
                    Status = "Active"
                },
                new User 
                { 
                    UserName = "user2", 
                    Email = "user2@example.com", 
                    PasswordHash = "hashed_password_3",
                    PhoneNumber = "+1234567892",
                    FullName = "Bob Smith",
                    ProfileImage = "https://example.com/user2.jpg",
                    Bio = "Looking for professional photos",
                    CreateAt = DateTime.Now.AddDays(-250),
                    UpdateAt = DateTime.Now,
                    Status = "Active"
                },
                
                // Photographers
                new User 
                { 
                    UserName = "photographer1", 
                    Email = "sarah@photography.com", 
                    PasswordHash = "hashed_password_4",
                    PhoneNumber = "+1234567893",
                    FullName = "Sarah Wilson",
                    ProfileImage = "https://example.com/photographer1.jpg",
                    Bio = "Professional portrait photographer with 8 years of experience",
                    CreateAt = DateTime.Now.AddDays(-200),
                    UpdateAt = DateTime.Now,
                    Status = "Active"
                },
                new User 
                { 
                    UserName = "photographer2", 
                    Email = "mike@photography.com", 
                    PasswordHash = "hashed_password_5",
                    PhoneNumber = "+1234567894",
                    FullName = "Mike Chen",
                    ProfileImage = "https://example.com/photographer2.jpg",
                    Bio = "Wedding and event photographer specializing in candid moments",
                    CreateAt = DateTime.Now.AddDays(-180),
                    UpdateAt = DateTime.Now,
                    Status = "Active"
                },
                new User 
                { 
                    UserName = "photographer3", 
                    Email = "emma@photography.com", 
                    PasswordHash = "hashed_password_6",
                    PhoneNumber = "+1234567895",
                    FullName = "Emma Davis",
                    ProfileImage = "https://example.com/photographer3.jpg",
                    Bio = "Landscape and nature photographer",
                    CreateAt = DateTime.Now.AddDays(-150),
                    UpdateAt = DateTime.Now,
                    Status = "Active"
                },
                
                // Location Owners
                new User 
                { 
                    UserName = "locationowner1", 
                    Email = "studio@downtown.com", 
                    PasswordHash = "hashed_password_7",
                    PhoneNumber = "+1234567896",
                    FullName = "Downtown Studio",
                    ProfileImage = "https://example.com/studio1.jpg",
                    Bio = "Professional photography studio in downtown",
                    CreateAt = DateTime.Now.AddDays(-120),
                    UpdateAt = DateTime.Now,
                    Status = "Active"
                },
                new User 
                { 
                    UserName = "locationowner2", 
                    Email = "garden@nature.com", 
                    PasswordHash = "hashed_password_8",
                    PhoneNumber = "+1234567897",
                    FullName = "Nature Garden",
                    ProfileImage = "https://example.com/garden1.jpg",
                    Bio = "Beautiful garden location for outdoor photography",
                    CreateAt = DateTime.Now.AddDays(-100),
                    UpdateAt = DateTime.Now,
                    Status = "Active"
                },
                
                // Moderators
                new User 
                { 
                    UserName = "moderator1", 
                    Email = "moderator@snaplink.com", 
                    PasswordHash = "hashed_password_9",
                    PhoneNumber = "+1234567898",
                    FullName = "Lisa Moderator",
                    ProfileImage = "https://example.com/moderator1.jpg",
                    Bio = "Content moderator",
                    CreateAt = DateTime.Now.AddDays(-90),
                    UpdateAt = DateTime.Now,
                    Status = "Active"
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
            return users;
        }

        private static void SeedUserRoles(SnaplinkDbContext context, List<User> users, List<Role> roles)
        {
            var userRoles = new List<UserRole>
            {
                new UserRole { UserId = users[0].UserId, RoleId = roles[0].RoleId, AssignedAt = DateTime.Now },
                new UserRole { UserId = users[1].UserId, RoleId = roles[1].RoleId, AssignedAt = DateTime.Now },
                new UserRole { UserId = users[2].UserId, RoleId = roles[1].RoleId, AssignedAt = DateTime.Now },
                new UserRole { UserId = users[3].UserId, RoleId = roles[2].RoleId, AssignedAt = DateTime.Now },
                new UserRole { UserId = users[4].UserId, RoleId = roles[2].RoleId, AssignedAt = DateTime.Now },
                new UserRole { UserId = users[5].UserId, RoleId = roles[2].RoleId, AssignedAt = DateTime.Now },
                new UserRole { UserId = users[6].UserId, RoleId = roles[3].RoleId, AssignedAt = DateTime.Now },
                new UserRole { UserId = users[7].UserId, RoleId = roles[3].RoleId, AssignedAt = DateTime.Now },
                new UserRole { UserId = users[8].UserId, RoleId = roles[4].RoleId, AssignedAt = DateTime.Now }
            };

            context.UserRoles.AddRange(userRoles);
            context.SaveChanges();
        }

        private static void SeedAdministrators(SnaplinkDbContext context, List<User> users)
        {
            var administrators = new List<Administrator>
            {
                new Administrator 
                { 
                    UserId = users[0].UserId, 
                    AccessLevel = "SuperAdmin", 
                    Department = "IT" 
                }
            };

            context.Administrators.AddRange(administrators);
            context.SaveChanges();
        }

        private static void SeedModerators(SnaplinkDbContext context, List<User> users)
        {
            var moderators = new List<Moderator>
            {
                new Moderator 
                { 
                    UserId = users[8].UserId, 
                    AreasOfFocus = "Content Moderation, User Support" 
                }
            };

            context.Moderators.AddRange(moderators);
            context.SaveChanges();
        }

        private static List<Photographer> SeedPhotographers(SnaplinkDbContext context, List<User> users)
        {
            var photographers = new List<Photographer>
            {
                new Photographer 
                { 
                    UserId = users[3].UserId,
                    YearsExperience = 8,
                    Equipment = "Canon EOS R5, 24-70mm f/2.8, 85mm f/1.4",
                    Specialty = "Portrait Photography",
                    PortfolioUrl = "https://sarahwilson.photography",
                    HourlyRate = 150.00m,
                    AvailabilityStatus = "Available",
                    Rating = 4.8m,
                    RatingSum = 24.0m,
                    RatingCount = 5,
                    FeaturedStatus = true,
                    VerificationStatus = "Verified"
                },
                new Photographer 
                { 
                    UserId = users[4].UserId,
                    YearsExperience = 5,
                    Equipment = "Sony A7III, 35mm f/1.4, 70-200mm f/2.8",
                    Specialty = "Wedding Photography",
                    PortfolioUrl = "https://mikechen.photography",
                    HourlyRate = 200.00m,
                    AvailabilityStatus = "Available",
                    Rating = 4.9m,
                    RatingSum = 49.0m,
                    RatingCount = 10,
                    FeaturedStatus = true,
                    VerificationStatus = "Verified"
                },
                new Photographer 
                { 
                    UserId = users[5].UserId,
                    YearsExperience = 3,
                    Equipment = "Nikon Z6, 14-24mm f/2.8, 24-70mm f/2.8",
                    Specialty = "Landscape Photography",
                    PortfolioUrl = "https://emmadavis.photography",
                    HourlyRate = 120.00m,
                    AvailabilityStatus = "Available",
                    Rating = 4.6m,
                    RatingSum = 23.0m,
                    RatingCount = 5,
                    FeaturedStatus = false,
                    VerificationStatus = "Pending"
                }
            };

            context.Photographers.AddRange(photographers);
            context.SaveChanges();
            return photographers;
        }

        private static void SeedPhotographerStyles(SnaplinkDbContext context, List<Photographer> photographers, List<Style> styles)
        {
            var photographerStyles = new List<PhotographerStyle>
            {
                new PhotographerStyle { PhotographerId = photographers[0].PhotographerId, StyleId = styles[0].StyleId },
                new PhotographerStyle { PhotographerId = photographers[0].PhotographerId, StyleId = styles[4].StyleId },
                new PhotographerStyle { PhotographerId = photographers[1].PhotographerId, StyleId = styles[1].StyleId },
                new PhotographerStyle { PhotographerId = photographers[1].PhotographerId, StyleId = styles[0].StyleId },
                new PhotographerStyle { PhotographerId = photographers[2].PhotographerId, StyleId = styles[2].StyleId },
                new PhotographerStyle { PhotographerId = photographers[2].PhotographerId, StyleId = styles[6].StyleId }
            };

            context.PhotographerStyles.AddRange(photographerStyles);
            context.SaveChanges();
        }

        private static void SeedPhotographerWallets(SnaplinkDbContext context, List<Photographer> photographers)
        {
            var wallets = new List<PhotographerWallet>
            {
                new PhotographerWallet { PhotographerId = photographers[0].PhotographerId, Balance = 1250.00m, UpdatedAt = DateTime.Now },
                new PhotographerWallet { PhotographerId = photographers[1].PhotographerId, Balance = 2100.00m, UpdatedAt = DateTime.Now },
                new PhotographerWallet { PhotographerId = photographers[2].PhotographerId, Balance = 750.00m, UpdatedAt = DateTime.Now }
            };

            context.PhotographerWallets.AddRange(wallets);
            context.SaveChanges();
        }

        private static List<LocationOwner> SeedLocationOwners(SnaplinkDbContext context, List<User> users)
        {
            var locationOwners = new List<LocationOwner>
            {
                new LocationOwner 
                { 
                    UserId = users[6].UserId,
                    BusinessName = "Downtown Photography Studio",
                    BusinessAddress = "123 Main St, Downtown, City",
                    BusinessRegistrationNumber = "BUS123456",
                    VerificationStatus = "Verified"
                },
                new LocationOwner 
                { 
                    UserId = users[7].UserId,
                    BusinessName = "Nature Garden Photography",
                    BusinessAddress = "456 Garden Ave, Suburb, City",
                    BusinessRegistrationNumber = "BUS789012",
                    VerificationStatus = "Verified"
                }
            };

            context.LocationOwners.AddRange(locationOwners);
            context.SaveChanges();
            return locationOwners;
        }

        private static List<Location> SeedLocations(SnaplinkDbContext context, List<LocationOwner> locationOwners)
        {
            var locations = new List<Location>
            {
                new Location 
                { 
                    LocationOwnerId = locationOwners[0].LocationOwnerId,
                    Name = "Downtown Studio A",
                    Description = "Professional photography studio with natural lighting",
                    Address = "123 Main St, Downtown, City",
                    Capacity = 20,
                    HourlyRate = 80.00m,
                    Indoor = true,
                    Outdoor = false,
                    Amenities = "Professional lighting, Backdrops, Changing room, WiFi",
                    AvailabilityStatus = "Available",
                    VerificationStatus = "Verified",
                    FeaturedStatus = true,
                    CreatedAt = DateTime.Now.AddDays(-80),
                    UpdatedAt = DateTime.Now
                },
                new Location 
                { 
                    LocationOwnerId = locationOwners[0].LocationOwnerId,
                    Name = "Downtown Studio B",
                    Description = "Large studio space for group photography",
                    Address = "123 Main St, Downtown, City",
                    Capacity = 50,
                    HourlyRate = 120.00m,
                    Indoor = true,
                    Outdoor = false,
                    Amenities = "Professional lighting, Multiple backdrops, Green screen, WiFi",
                    AvailabilityStatus = "Available",
                    VerificationStatus = "Verified",
                    FeaturedStatus = false,
                    CreatedAt = DateTime.Now.AddDays(-70),
                    UpdatedAt = DateTime.Now
                },
                new Location 
                { 
                    LocationOwnerId = locationOwners[1].LocationOwnerId,
                    Name = "Garden Pavilion",
                    Description = "Beautiful outdoor garden with natural scenery",
                    Address = "456 Garden Ave, Suburb, City",
                    Capacity = 30,
                    HourlyRate = 60.00m,
                    Indoor = false,
                    Outdoor = true,
                    Amenities = "Garden paths, Water features, Seating areas, Parking",
                    AvailabilityStatus = "Available",
                    VerificationStatus = "Verified",
                    FeaturedStatus = true,
                    CreatedAt = DateTime.Now.AddDays(-60),
                    UpdatedAt = DateTime.Now
                }
            };

            context.Locations.AddRange(locations);
            context.SaveChanges();
            return locations;
        }

        private static void SeedLocationImages(SnaplinkDbContext context, List<Location> locations)
        {
            var locationImages = new List<LocationImage>
            {
                new LocationImage 
                { 
                    LocationId = locations[0].LocationId,
                    ImageUrl = "https://example.com/studio-a-1.jpg",
                    Caption = "Main studio area",
                    IsPrimary = true,
                    UploadedAt = DateTime.Now.AddDays(-75)
                },
                new LocationImage 
                { 
                    LocationId = locations[0].LocationId,
                    ImageUrl = "https://example.com/studio-a-2.jpg",
                    Caption = "Lighting setup",
                    IsPrimary = false,
                    UploadedAt = DateTime.Now.AddDays(-74)
                },
                new LocationImage 
                { 
                    LocationId = locations[1].LocationId,
                    ImageUrl = "https://example.com/studio-b-1.jpg",
                    Caption = "Large studio space",
                    IsPrimary = true,
                    UploadedAt = DateTime.Now.AddDays(-65)
                },
                new LocationImage 
                { 
                    LocationId = locations[2].LocationId,
                    ImageUrl = "https://example.com/garden-1.jpg",
                    Caption = "Garden entrance",
                    IsPrimary = true,
                    UploadedAt = DateTime.Now.AddDays(-55)
                },
                new LocationImage 
                { 
                    LocationId = locations[2].LocationId,
                    ImageUrl = "https://example.com/garden-2.jpg",
                    Caption = "Water feature area",
                    IsPrimary = false,
                    UploadedAt = DateTime.Now.AddDays(-54)
                }
            };

            context.LocationImages.AddRange(locationImages);
            context.SaveChanges();
        }

        private static void SeedAdvertisements(SnaplinkDbContext context, List<Location> locations)
        {
            var advertisements = new List<Advertisement>
            {
                new Advertisement 
                { 
                    LocationId = locations[0].LocationId,
                    Title = "Professional Studio Available",
                    Description = "Book our professional studio for your next photo shoot",
                    ImageUrl = "https://example.com/studio-ad.jpg",
                    StartDate = DateTime.Now.AddDays(-30),
                    EndDate = DateTime.Now.AddDays(30),
                    Cost = 500.00m,
                    Status = "Active"
                },
                new Advertisement 
                { 
                    LocationId = locations[2].LocationId,
                    Title = "Garden Photography Special",
                    Description = "Beautiful outdoor location for nature photography",
                    ImageUrl = "https://example.com/garden-ad.jpg",
                    StartDate = DateTime.Now.AddDays(-20),
                    EndDate = DateTime.Now.AddDays(40),
                    Cost = 300.00m,
                    Status = "Active"
                }
            };

            context.Advertisements.AddRange(advertisements);
            context.SaveChanges();
        }

        private static List<Booking> SeedBookings(SnaplinkDbContext context, List<User> users, List<Photographer> photographers, List<Location> locations)
        {
            var bookings = new List<Booking>
            {
                new Booking 
                { 
                    UserId = users[1].UserId,
                    PhotographerId = photographers[0].PhotographerId,
                    LocationId = locations[0].LocationId,
                    StartDatetime = DateTime.Now.AddDays(7),
                    EndDatetime = DateTime.Now.AddDays(7).AddHours(2),
                    TotalPrice = 460.00m,
                    Status = "Confirmed",
                    SpecialRequests = "Natural lighting preferred",
                    CreatedAt = DateTime.Now.AddDays(-5),
                    UpdatedAt = DateTime.Now.AddDays(-5)
                },
                new Booking 
                { 
                    UserId = users[2].UserId,
                    PhotographerId = photographers[1].PhotographerId,
                    LocationId = locations[2].LocationId,
                    StartDatetime = DateTime.Now.AddDays(14),
                    EndDatetime = DateTime.Now.AddDays(14).AddHours(3),
                    TotalPrice = 780.00m,
                    Status = "Confirmed",
                    SpecialRequests = "Outdoor wedding photos",
                    CreatedAt = DateTime.Now.AddDays(-3),
                    UpdatedAt = DateTime.Now.AddDays(-3)
                },
                new Booking 
                { 
                    UserId = users[1].UserId,
                    PhotographerId = photographers[2].PhotographerId,
                    LocationId = locations[2].LocationId,
                    StartDatetime = DateTime.Now.AddDays(-10),
                    EndDatetime = DateTime.Now.AddDays(-10).AddHours(2),
                    TotalPrice = 360.00m,
                    Status = "Completed",
                    SpecialRequests = "Landscape photography",
                    CreatedAt = DateTime.Now.AddDays(-15),
                    UpdatedAt = DateTime.Now.AddDays(-10)
                }
            };

            context.Bookings.AddRange(bookings);
            context.SaveChanges();
            return bookings;
        }

        private static void SeedReviews(SnaplinkDbContext context, List<Booking> bookings)
        {
            var reviews = new List<Review>
            {
                new Review 
                { 
                    BookingId = bookings[2].BookingId,
                    ReviewerId = bookings[2].UserId,
                    RevieweeId = bookings[2].PhotographerId,
                    RevieweeType = "Photographer",
                    Rating = 5,
                    Comment = "Excellent landscape photography! Emma captured the beauty of nature perfectly.",
                    CreatedAt = DateTime.Now.AddDays(-8),
                    UpdatedAt = DateTime.Now.AddDays(-8)
                }
            };

            context.Reviews.AddRange(reviews);
            context.SaveChanges();
        }

        private static List<Payment> SeedPayments(SnaplinkDbContext context, List<Booking> bookings)
        {
            var payments = new List<Payment>
            {
                new Payment 
                { 
                    BookingId = bookings[0].BookingId,
                    Amount = 460.00m,
                    PaymentMethod = "Credit Card",
                    Status = "Completed",
                    TransactionId = "TXN001",
                    PlatformFee = 23.00m,
                    PhotographerPayoutAmount = 350.00m,
                    LocationOwnerPayoutAmount = 87.00m,
                    CreatedAt = DateTime.Now.AddDays(-5),
                    UpdatedAt = DateTime.Now.AddDays(-5)
                },
                new Payment 
                { 
                    BookingId = bookings[1].BookingId,
                    Amount = 780.00m,
                    PaymentMethod = "PayPal",
                    Status = "Completed",
                    TransactionId = "TXN002",
                    PlatformFee = 39.00m,
                    PhotographerPayoutAmount = 585.00m,
                    LocationOwnerPayoutAmount = 156.00m,
                    CreatedAt = DateTime.Now.AddDays(-3),
                    UpdatedAt = DateTime.Now.AddDays(-3)
                },
                new Payment 
                { 
                    BookingId = bookings[2].BookingId,
                    Amount = 360.00m,
                    PaymentMethod = "Credit Card",
                    Status = "Completed",
                    TransactionId = "TXN003",
                    PlatformFee = 18.00m,
                    PhotographerPayoutAmount = 270.00m,
                    LocationOwnerPayoutAmount = 72.00m,
                    CreatedAt = DateTime.Now.AddDays(-15),
                    UpdatedAt = DateTime.Now.AddDays(-10)
                }
            };

            context.Payments.AddRange(payments);
            context.SaveChanges();
            return payments;
        }

        private static void SeedPremiumSubscriptions(SnaplinkDbContext context, List<User> users, List<PremiumPackage> packages, List<Payment> payments)
        {
            var subscriptions = new List<PremiumSubscription>
            {
                new PremiumSubscription 
                { 
                    UserId = users[3].UserId,
                    PackageId = packages[1].PackageId,
                    PaymentId = null,
                    StartDate = DateTime.Now.AddDays(-30),
                    EndDate = DateTime.Now.AddDays(30),
                    Status = "Active"
                },
                new PremiumSubscription 
                { 
                    UserId = users[4].UserId,
                    PackageId = packages[1].PackageId,
                    PaymentId = null,
                    StartDate = DateTime.Now.AddDays(-15),
                    EndDate = DateTime.Now.AddDays(15),
                    Status = "Active"
                }
            };

            context.PremiumSubscriptions.AddRange(subscriptions);
            context.SaveChanges();
        }

        private static void SeedTransactions(SnaplinkDbContext context, List<User> users)
        {
            var transactions = new List<Transaction>
            {
                new Transaction 
                { 
                    UserId = users[1].UserId,
                    Amount = 460.00m,
                    Type = "Payment",
                    Status = "Completed",
                    Description = "Payment for photography booking",
                    CreatedAt = DateTime.Now.AddDays(-5)
                },
                new Transaction 
                { 
                    UserId = users[2].UserId,
                    Amount = 780.00m,
                    Type = "Payment",
                    Status = "Completed",
                    Description = "Payment for wedding photography",
                    CreatedAt = DateTime.Now.AddDays(-3)
                }
            };

            context.Transactions.AddRange(transactions);
            context.SaveChanges();
        }

        private static void SeedNotifications(SnaplinkDbContext context, List<User> users)
        {
            var notifications = new List<Notification>
            {
                new Notification 
                { 
                    UserId = users[1].UserId,
                    Title = "Booking Confirmed",
                    Content = "Your booking with Sarah Wilson has been confirmed for next week.",
                    NotificationType = "Booking",
                    ReferenceId = 1,
                    ReadStatus = false,
                    CreatedAt = DateTime.Now.AddDays(-5)
                },
                new Notification 
                { 
                    UserId = users[2].UserId,
                    Title = "Payment Successful",
                    Content = "Your payment of $780.00 has been processed successfully.",
                    NotificationType = "Payment",
                    ReferenceId = 2,
                    ReadStatus = true,
                    CreatedAt = DateTime.Now.AddDays(-3)
                },
                new Notification 
                { 
                    UserId = users[3].UserId,
                    Title = "New Booking Request",
                    Content = "You have received a new booking request from Alice Johnson.",
                    NotificationType = "Booking",
                    ReferenceId = 1,
                    ReadStatus = false,
                    CreatedAt = DateTime.Now.AddDays(-5)
                }
            };

            context.Notifications.AddRange(notifications);
            context.SaveChanges();
        }

        private static void SeedMessages(SnaplinkDbContext context, List<User> users)
        {
            var messages = new List<Messagess>
            {
                new Messagess 
                { 
                    SenderId = users[1].UserId,
                    RecipientId = users[3].UserId,
                    Content = "Hi Sarah, I'm looking forward to our photo session next week!",
                    ReadStatus = false,
                    CreatedAt = DateTime.Now.AddDays(-4)
                },
                new Messagess 
                { 
                    SenderId = users[3].UserId,
                    RecipientId = users[1].UserId,
                    Content = "Hi Alice! I'm excited too. I'll bring some great lighting equipment.",
                    ReadStatus = true,
                    CreatedAt = DateTime.Now.AddDays(-3)
                },
                new Messagess 
                { 
                    SenderId = users[2].UserId,
                    RecipientId = users[4].UserId,
                    Content = "Mike, can we discuss the wedding photography details?",
                    ReadStatus = false,
                    CreatedAt = DateTime.Now.AddDays(-2)
                }
            };

            context.Messagesses.AddRange(messages);
            context.SaveChanges();
        }

        private static void SeedComplaints(SnaplinkDbContext context, List<User> users, List<Booking> bookings)
        {
            var complaints = new List<Complaint>
            {
                new Complaint 
                { 
                    ReporterId = users[1].UserId,
                    ReportedUserId = users[3].UserId,
                    BookingId = bookings[0].BookingId,
                    ComplaintType = "Service Quality",
                    Description = "Photographer arrived late to the session",
                    Status = "Under Review",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    UpdatedAt = DateTime.Now.AddDays(-1)
                }
            };

            context.Complaints.AddRange(complaints);
            context.SaveChanges();
        }

        private static void SeedWithdrawalRequests(SnaplinkDbContext context, List<Photographer> photographers)
        {
            var withdrawalRequests = new List<WithdrawalRequest>
            {
                new WithdrawalRequest 
                { 
                    PhotographerId = photographers[0].PhotographerId,
                    Amount = 500.00m,
                    BankName = "City Bank",
                    BankAccountName = "Sarah Wilson",
                    BankAccountNumber = "1234567890",
                    RequestStatus = "Pending",
                    RequestedAt = DateTime.Now.AddDays(-2)
                },
                new WithdrawalRequest 
                { 
                    PhotographerId = photographers[1].PhotographerId,
                    Amount = 1000.00m,
                    BankName = "National Bank",
                    BankAccountName = "Mike Chen",
                    BankAccountNumber = "0987654321",
                    RequestStatus = "Approved",
                    RequestedAt = DateTime.Now.AddDays(-5),
                    ProcessedAt = DateTime.Now.AddDays(-3)
                }
            };

            context.WithdrawalRequests.AddRange(withdrawalRequests);
            context.SaveChanges();
        }
    }
} 