using System;
using System.Linq;
using SnapLink_Repository.DBContext;
using SnapLink_Repository.Entity;

namespace SnapLink_Repository.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SnaplinkDbContext context)
        {
            Console.WriteLine("=== Starting Database Initialization ===");
            
            context.Database.EnsureCreated();
            Console.WriteLine("Database created/ensured successfully");

            // Seed Roles
            if (!context.Roles.Any())
            {
                Console.WriteLine("Seeding Roles...");
                var roles = new[]
                {
                    new Role { RoleName = "Admin", RoleDescription = "Administrator" },
                    new Role { RoleName = "Photographer", RoleDescription = "Professional Photographer" },
                    new Role { RoleName = "User", RoleDescription = "Regular User" },
                    new Role { RoleName = "Moderator", RoleDescription = "Moderator" },
                    new Role { RoleName = "Owner", RoleDescription = "Location Owner" }
                };
                context.Roles.AddRange(roles);
                context.SaveChanges();
                Console.WriteLine($"Roles seeded successfully: {roles.Length} roles added");
            }
            else
            {
                Console.WriteLine("Roles already exist, skipping...");
            }

            // Seed Users
            if (!context.Users.Any())
            {
                Console.WriteLine("Seeding Users...");
                var users = new[]
                {
                    new User { UserName = "SnaplinkAI", Email = "SnaplinkAI@example.com", PasswordHash = "Snaplink", PhoneNumber = "", FullName = "Snap link", Status = "Active", IsVerified = true, CreateAt = DateTime.Now, ProfileImage ="https://snaplinkstorage.blob.core.windows.net/snaplinkstorageblob/user/1/AISUPPORT.png" },
                    new User { UserName = "linkka", Email = "linkka@example.com", PasswordHash = "hash2", PhoneNumber = "1234567891", FullName = "linhka", Status = "Active", IsVerified = true, CreateAt = DateTime.Now,ProfileImage ="https://snaplinkstorage.blob.core.windows.net/snaplinkstorageblob/photographer/2/20250715052645_linggka109_3670740457001717233_s2025-7-15-12.25.361_story.jpg" },
                    new User { UserName = "alice", Email = "alice@example.com", PasswordHash = "hash1", PhoneNumber = "1234567890", FullName = "Linh", Status = "Active", IsVerified = true, CreateAt = DateTime.Now,ProfileImage ="https://snaplinkstorage.blob.core.windows.net/snaplinkstorageblob/photographer/2/linggka109_2933653585994336284's2025-7-27-20.14.485%20story.jpg" },
                    new User { UserName = "carol", Email = "carol@example.com", PasswordHash = "hash3", PhoneNumber = "1234567892", FullName = "Chu Diệu Linh", Status = "Active", IsVerified = true, CreateAt = DateTime.Now,ProfileImage ="https://snaplinkstorage.blob.core.windows.net/snaplinkstorageblob/photographer/2/linggka109_3691773221381401224's2025-8-9-3.40.579%20story.jpg" },
                    new User { UserName = "dave", Email = "dave@example.com", PasswordHash = "hash4", PhoneNumber = "1234567893", FullName = "Dave Brown", Status = "Active", IsVerified = true, CreateAt = DateTime.Now,ProfileImage ="https://picsum.photos/200/300?random=1" },
                    new User { UserName = "eve", Email = "eve@example.com", PasswordHash = "hash5", PhoneNumber = "1234567894", FullName = "Eve Black", Status = "Active", IsVerified = true, CreateAt = DateTime.Now,ProfileImage ="https://picsum.photos/200/300?random=2" },
                    new User { UserName = "frank", Email = "frank@example.com", PasswordHash = "hash6", PhoneNumber = "1234567895", FullName = "Frank Miller", Status = "Active", IsVerified = true, CreateAt = DateTime.Now,ProfileImage ="https://picsum.photos/200/300?random=3" },
                    new User { UserName = "grace", Email = "grace@example.com", PasswordHash = "hash7", PhoneNumber = "1234567896", FullName = "Grace Lee", Status = "Active", IsVerified = true, CreateAt = DateTime.Now,ProfileImage ="https://picsum.photos/200/300?random=4" },
                    new User { UserName = "henry", Email = "henry@example.com", PasswordHash = "hash8", PhoneNumber = "1234567897", FullName = "Henry Wilson", Status = "Active", IsVerified = true, CreateAt = DateTime.Now,ProfileImage ="https://picsum.photos/200/300?random=5" },
                    new User { UserName = "iris", Email = "iris@example.com", PasswordHash = "hash9", PhoneNumber = "1234567898", FullName = "Iris Davis", Status = "Active", IsVerified = true, CreateAt = DateTime.Now,ProfileImage ="https://picsum.photos/200/300?random=6" },
                    new User { UserName = "jack", Email = "jack@example.com", PasswordHash = "hash10", PhoneNumber = "1234567899", FullName = "Jack Taylor", Status = "Active", IsVerified = true, CreateAt = DateTime.Now,ProfileImage ="https://picsum.photos/200/300?random=7" },
                    new User { UserName = "kate", Email = "kate@example.com", PasswordHash = "hash11", PhoneNumber = "1234567900", FullName = "Kate Anderson", Status = "Active", IsVerified = true, CreateAt = DateTime.Now ,ProfileImage ="https://picsum.photos/200/300?random=8"},
                    new User { UserName = "leo", Email = "leo@example.com", PasswordHash = "hash12", PhoneNumber = "1234567901", FullName = "Leo Martinez", Status = "Active", IsVerified = true, CreateAt = DateTime.Now,ProfileImage ="https://picsum.photos/200/300?random=9" },
                    new User { UserName = "maya", Email = "maya@example.com", PasswordHash = "hash13", PhoneNumber = "1234567902", FullName = "Maya Rodriguez", Status = "Active", IsVerified = true, CreateAt = DateTime.Now ,ProfileImage ="https://picsum.photos/200/300?random=10"},
                    new User { UserName = "nina", Email = "nina@example.com", PasswordHash = "hash14", PhoneNumber = "1234567903", FullName = "Nina Thompson", Status = "Active", IsVerified = true, CreateAt = DateTime.Now,ProfileImage ="https://picsum.photos/200/300?random=11" },
                    new User { UserName = "oscar", Email = "oscar@example.com", PasswordHash = "hash15", PhoneNumber = "1234567904", FullName = "Oscar Garcia", Status = "Active", IsVerified = true, CreateAt = DateTime.Now,ProfileImage ="https://picsum.photos/200/300?random=12" },
                    // Additional users for location owners
                    new User { UserName = "peter", Email = "peter@example.com", PasswordHash = "hash16", PhoneNumber = "1234567905", FullName = "Peter Chen", Status = "Active", IsVerified = true, CreateAt = DateTime.Now,ProfileImage ="https://picsum.photos/200/300?random=13" },
                    new User { UserName = "sarah", Email = "sarah@example.com", PasswordHash = "hash17", PhoneNumber = "1234567906", FullName = "Sarah Kim", Status = "Active", IsVerified = true, CreateAt = DateTime.Now,ProfileImage ="https://picsum.photos/200/300?random=14" },
                    new User { UserName = "mike", Email = "mike@example.com", PasswordHash = "hash18", PhoneNumber = "1234567907", FullName = "Mike Johnson", Status = "Active", IsVerified = true, CreateAt = DateTime.Now,ProfileImage ="https://picsum.photos/200/300?random=15" },
                    new User { UserName = "lisa", Email = "lisa@example.com", PasswordHash = "hash19", PhoneNumber = "1234567908", FullName = "Lisa Wang", Status = "Active", IsVerified = true, CreateAt = DateTime.Now,ProfileImage ="https://picsum.photos/200/300?random=16" },
                    new User { UserName = "david", Email = "david@example.com", PasswordHash = "hash20", PhoneNumber = "1234567909", FullName = "David Park", Status = "Active", IsVerified = true, CreateAt = DateTime.Now,ProfileImage ="https://picsum.photos/200/300?random=17" }
                };
                context.Users.AddRange(users);
                context.SaveChanges();
                Console.WriteLine($"Users seeded successfully: {users.Length} users added");
            }
            else
            {
                Console.WriteLine("Users already exist, skipping...");
            }

            // Seed UserRoles
            if (!context.UserRoles.Any())
            {
                Console.WriteLine("Seeding UserRoles...");
                var userRoles = new List<UserRole>();
                
                // Get role IDs once to avoid repeated database queries
                var userRoleId = context.Roles.First(r => r.RoleName == "User").RoleId;
                var ownerRoleId = context.Roles.First(r => r.RoleName == "Owner").RoleId;
                var PhotographerRoleId = context.Roles.First(r => r.RoleName == "Photographer").RoleId;
                // Add User role for first 20 users (0-19)
                for (int i = 0; i < 20; i++)
                {
                    userRoles.Add(new UserRole 
                    { 
                        UserId = context.Users.Skip(i).First().UserId, 
                        RoleId = userRoleId 
                    });
                }
                for (int i = 1; i < 15; i++)
                {
                    userRoles.Add(new UserRole
                    {
                        UserId = context.Users.Skip(i).First().UserId,
                        RoleId = PhotographerRoleId
                    });
                }
                // Add Owner role for users 15-19 (Peter, Sarah, Mike, Lisa, David)
                for (int i = 1; i < 20; i++)
                {
                    userRoles.Add(new UserRole 
                    { 
                        UserId = context.Users.Skip(i).First().UserId, 
                        RoleId = ownerRoleId 
                    });
                }
                
                context.UserRoles.AddRange(userRoles);
                context.SaveChanges();
                Console.WriteLine($"UserRoles seeded successfully: {userRoles.Count} user roles added");
            }
            else
            {
                Console.WriteLine("UserRoles already exist, skipping...");
            }

            // Seed Styles
            if (!context.Styles.Any())
            {
                Console.WriteLine("Seeding Styles...");
                var styles = new[]
                {
                    new Style { Name = "Nàng thơ", Description = "Nhẹ nhàng, lãng mạn, bay bổng" },
                    new Style { Name = "Dễ thương", Description = "Ngọt ngào, vui tươi, đáng yêu" },
                    new Style { Name = "Mạnh mẽ", Description = "Cá tính, tự tin, quyết đoán" },
                    new Style { Name = "Cổ điển", Description = "Thanh lịch, sang trọng, hoài niệm" },
                    new Style { Name = "Hiện đại", Description = "Năng động, trẻ trung, tinh tế" },
                    new Style { Name = "Tối giản", Description = "Đơn giản, tinh gọn, tinh tế" },
                    new Style { Name = "Quý phái", Description = "Quý phái, đẳng cấp, thu hút" }
                };
                context.Styles.AddRange(styles);
                context.SaveChanges();
                Console.WriteLine($"Styles seeded successfully: {styles.Length} styles added");
            }
            else
            {
                Console.WriteLine("Styles already exist, skipping...");
            }

            // Seed Photographers
            if (!context.Photographers.Any())
            {
                Console.WriteLine("Seeding Photographers...");
                var users = context.Users.Take(16).ToList();
                var photographers = new[]
                {
            new Photographer { UserId = users[1].UserId, YearsExperience = 3, Equipment = "Iphone 19 Pro Max", HourlyRate = 5000, AvailabilityStatus = "Available", Rating = 4.67M, RatingCount = 12, RatingSum = 56,Latitude = 10.865958 ,Longitude =106.802917 },
            new Photographer { UserId = users[2].UserId, YearsExperience = 7, Equipment = "Iphone 18 Pro Max", HourlyRate = 120000, AvailabilityStatus = "Busy", Rating = 3.28M, RatingCount = 25, RatingSum = 82,Latitude = 10.780432, Longitude = 106.730128 },
            new Photographer { UserId = users[3].UserId, YearsExperience = 2, Equipment = "Iphone 17 Pro Max", HourlyRate = 70000, AvailabilityStatus = "Available", Rating = 4.05M, RatingCount = 37, RatingSum = 150,Latitude = 10.860129, Longitude = 106.789432 },
            new Photographer { UserId = users[4].UserId, YearsExperience = 4, Equipment = "Iphone 6 Pro Max", HourlyRate = 90000, AvailabilityStatus = "Available", Rating = 3.00M, RatingCount = 9, RatingSum = 27,Latitude = 10.950876, Longitude = 106.843219 },
            new Photographer { UserId = users[5].UserId, YearsExperience = 6, Equipment = "Iphone 15 Pro Max", HourlyRate = 110000, AvailabilityStatus = "Available", Rating = 4.80M, RatingCount = 44, RatingSum = 211,Latitude = 10.820567, Longitude = 106.990321 },
            new Photographer { UserId = users[6].UserId, YearsExperience = 4, Equipment = "Iphone 15 Pro Max", HourlyRate = 85000, AvailabilityStatus = "Available", Rating = 3.44M, RatingCount = 18, RatingSum = 62,Latitude = 10.915678, Longitude = 106.920543},
            new Photographer { UserId = users[7].UserId, YearsExperience = 8, Equipment = "Iphone 15 Pro Max", HourlyRate = 130000, AvailabilityStatus = "Busy", Rating = 4.38M, RatingCount = 21, RatingSum = 92,Latitude = 10.720345, Longitude = 106.850456 },
            new Photographer { UserId = users[8].UserId, YearsExperience = 3, Equipment = "Iphone 15 Pro Max", HourlyRate = 75000, AvailabilityStatus = "Available", Rating = 3.00M, RatingCount = 15, RatingSum = 45,Latitude = 10.910876, Longitude = 106.700987 },
            new Photographer { UserId = users[9].UserId, YearsExperience = 5, Equipment = "Iphone 15 Pro Max", HourlyRate = 95000, AvailabilityStatus = "Available", Rating = 3.88M, RatingCount = 33, RatingSum = 128,Latitude = 10.770654, Longitude = 106.810543 },
            new Photographer { UserId = users[10].UserId, YearsExperience = 7, Equipment = "Iphone 15 Pro Max", HourlyRate = 105000, AvailabilityStatus = "Available", Rating = 4.79M, RatingCount = 28, RatingSum = 134,Latitude = 10.895432, Longitude = 106.765432},
            new Photographer { UserId = users[11].UserId, YearsExperience = 2, Equipment = "Iphone 15 Pro Max", HourlyRate = 65000, AvailabilityStatus = "Available", Rating = 3.64M, RatingCount = 11, RatingSum = 40,Latitude = 10.730987, Longitude = 106.925678},
            new Photographer { UserId = users[12].UserId, YearsExperience = 6, Equipment = "Iphone 15 Pro Max", HourlyRate = 115000, AvailabilityStatus = "Busy", Rating = 4.68M, RatingCount = 19, RatingSum = 89,Latitude = 10.960432, Longitude = 106.780321 },
            new Photographer { UserId = users[13].UserId, YearsExperience = 4, Equipment = "Iphone 15 Pro Max", HourlyRate = 80000, AvailabilityStatus = "Available", Rating = 4.21M, RatingCount = 47, RatingSum = 198,Latitude = 10.805678, Longitude = 106.880987 },
            new Photographer { UserId = users[14].UserId, YearsExperience = 9, Equipment = "Iphone 15 Pro Max", HourlyRate = 140000, AvailabilityStatus = "Available", Rating = 3.00M, RatingCount = 23, RatingSum = 69 ,Latitude = 10.875321, Longitude = 106.950432 },
            new Photographer { UserId = users[15].UserId, YearsExperience = 9, Equipment = "Iphone 15 Pro Max", HourlyRate = 140000, AvailabilityStatus = "Available", Rating = 3.94M, RatingCount = 31, RatingSum = 122,Latitude = 10.741234, Longitude = 106.845678  },

                };
                context.Photographers.AddRange(photographers);
                context.SaveChanges();
                Console.WriteLine($"Photographers seeded successfully: {photographers.Length} photographers added");
            }

            // Seed PhotographerStyles
            if (!context.PhotographerStyles.Any())
            {
                Console.WriteLine("Seeding PhotographerStyles...");
                var photographers = context.Photographers.Take(15).ToList();
                var styles = context.Styles.Take(5).ToList();
                var photographerStyles = new[]
                {                    
                    // Photographer 2 (Bob) - Landscape specialist  
                    new PhotographerStyle { PhotographerId = photographers[1].PhotographerId, StyleId = styles[1].StyleId },
                    
                    // Photographer 3 (Carol) - Wedding specialist
                    new PhotographerStyle { PhotographerId = photographers[2].PhotographerId, StyleId = styles[2].StyleId },
                    
                    // Photographer 4 (Dave) - Event specialist
                    new PhotographerStyle { PhotographerId = photographers[3].PhotographerId, StyleId = styles[3].StyleId },
                    
                    // Photographer 5 (Eve) - Fashion specialist
                    new PhotographerStyle { PhotographerId = photographers[4].PhotographerId, StyleId = styles[4].StyleId },
                    
                    // Photographer 6 (Frank) - Portrait + Fashion (2 styles)
                    new PhotographerStyle { PhotographerId = photographers[5].PhotographerId, StyleId = styles[0].StyleId },
                    new PhotographerStyle { PhotographerId = photographers[5].PhotographerId, StyleId = styles[4].StyleId },
                    
                    // Photographer 7 (Grace) - Landscape + Event (2 styles)
                    new PhotographerStyle { PhotographerId = photographers[6].PhotographerId, StyleId = styles[1].StyleId },
                    new PhotographerStyle { PhotographerId = photographers[6].PhotographerId, StyleId = styles[3].StyleId },
                    
                    // Photographer 8 (Henry) - Wedding + Portrait + Event (3 styles)
                    new PhotographerStyle { PhotographerId = photographers[7].PhotographerId, StyleId = styles[2].StyleId },
                    new PhotographerStyle { PhotographerId = photographers[7].PhotographerId, StyleId = styles[0].StyleId },
                    new PhotographerStyle { PhotographerId = photographers[7].PhotographerId, StyleId = styles[3].StyleId },
                    
                    // Photographer 9 (Iris) - Event specialist
                    new PhotographerStyle { PhotographerId = photographers[8].PhotographerId, StyleId = styles[3].StyleId },
                    
                    // Photographer 10 (Jack) - Fashion + Portrait (2 styles)
                    new PhotographerStyle { PhotographerId = photographers[9].PhotographerId, StyleId = styles[4].StyleId },
                    new PhotographerStyle { PhotographerId = photographers[9].PhotographerId, StyleId = styles[0].StyleId },
                    
                    // Photographer 11 (Kate) - Portrait + Wedding (2 styles)
                    new PhotographerStyle { PhotographerId = photographers[10].PhotographerId, StyleId = styles[0].StyleId },
                    new PhotographerStyle { PhotographerId = photographers[10].PhotographerId, StyleId = styles[2].StyleId },
                    
                    // Photographer 12 (Leo) - Landscape specialist
                    new PhotographerStyle { PhotographerId = photographers[11].PhotographerId, StyleId = styles[1].StyleId },
                    
                    // Photographer 13 (Maya) - Wedding + Fashion + Event (3 styles)
                    new PhotographerStyle { PhotographerId = photographers[12].PhotographerId, StyleId = styles[2].StyleId },
                    new PhotographerStyle { PhotographerId = photographers[12].PhotographerId, StyleId = styles[4].StyleId },
                    new PhotographerStyle { PhotographerId = photographers[12].PhotographerId, StyleId = styles[3].StyleId },
                    
                    // Photographer 14 (Nina) - Event + Landscape (2 styles)
                    new PhotographerStyle { PhotographerId = photographers[13].PhotographerId, StyleId = styles[3].StyleId },
                    new PhotographerStyle { PhotographerId = photographers[13].PhotographerId, StyleId = styles[1].StyleId },
                    
                    // Photographer 15 (Oscar) - Fashion + Portrait + Wedding (3 styles)
                    new PhotographerStyle { PhotographerId = photographers[14].PhotographerId, StyleId = styles[4].StyleId },
                    new PhotographerStyle { PhotographerId = photographers[14].PhotographerId, StyleId = styles[0].StyleId },
                    new PhotographerStyle { PhotographerId = photographers[14].PhotographerId, StyleId = styles[2].StyleId }
                };
                context.PhotographerStyles.AddRange(photographerStyles);
                context.SaveChanges();
                Console.WriteLine($"PhotographerStyles seeded successfully: {photographerStyles.Length} photographer styles added");
            }

            // Seed LocationOwners
            if (!context.LocationOwners.Any())
            {
                Console.WriteLine("Seeding LocationOwners...");
                var users = context.Users.Take(12).ToList(); // Take 10 users (5 original + 5 new)
                var owners = new[]
                {
                    new LocationOwner { UserId = users[1].UserId, BusinessName = "EventSpaces", BusinessAddress = "eventspaces@example.com" },
                    new LocationOwner { UserId = users[2].UserId, BusinessName = "PhotoStudios", BusinessAddress = "photostudios@example.com" },
                    new LocationOwner { UserId = users[3].UserId, BusinessName = "UrbanVenues", BusinessAddress = "urbanvenues@example.com" },
                    new LocationOwner { UserId = users[4].UserId, BusinessName = "NatureSpots", BusinessAddress = "naturespots@example.com" },
                    // Additional location owners
                    new LocationOwner { UserId = users[5].UserId, BusinessName = "Studio Elite", BusinessAddress = "studioelite@example.com" },
                    new LocationOwner { UserId = users[6].UserId, BusinessName = "Creative Spaces", BusinessAddress = "creativespaces@example.com" },
                    new LocationOwner { UserId = users[7].UserId, BusinessName = "Modern Venues", BusinessAddress = "modernvenues@example.com" },
                    new LocationOwner { UserId = users[8].UserId, BusinessName = "Premium Studios", BusinessAddress = "premiumstudios@example.com" },
                    new LocationOwner { UserId = users[9].UserId, BusinessName = "Luxury Locations", BusinessAddress = "luxurylocations@example.com" },
                    new LocationOwner { UserId = users[10].UserId, BusinessName = "Luxury Locations 2", BusinessAddress = "luxurylocations2@example.com" },
                    new LocationOwner { UserId = users[11].UserId, BusinessName = "Luxury Locations 3", BusinessAddress = "luxurylocations3@example.com" }
                };
                context.LocationOwners.AddRange(owners);
                context.SaveChanges();
                Console.WriteLine($"LocationOwners seeded successfully: {owners.Length} location owners added");
            }

            // Seed Locations
            if (!context.Locations.Any())
            {
                Console.WriteLine("Seeding Locations...");
                var owners = context.LocationOwners.Take(13).ToList(); // Take all 10 location owners
                var locations = new[]
                {
                    // Original 5 registered locations with fees
                    new Location { 
                        Name = "Quảng Trường Sáng Tạo - ĐHQG TPHCM", 
                        Address = "Đ. Quảng Trường Sáng Tạo, Đông Hoà, Dĩ An, Bình Dương 75306, Việt Nam", 
                        Description = "Quảng Trường Sáng Tạo", 
                        HourlyRate = 0, 
                        Capacity = 20, 
                        Indoor = false, 
                        Outdoor = true, 
                        AvailabilityStatus = "Available", 
                        LocationType = "Registered",
                        ExternalPlaceId = "ChIJaXQRs6lZwokRY6EFpJnhNNE", // Example Google Places ID
                        CreatedAt = DateTime.Now,
                        Latitude = 10.8735842,
                        Longitude = 106.8025134

                    },
                    new Location { 
                        LocationOwnerId = owners[1].LocationOwnerId, 
                        Name = "Riverside Venue", 
                        Address = "456 River Rd, New York, NY", 
                        Description = "Beautiful riverside location", 
                        HourlyRate = 70, 
                        Capacity = 50, 
                        Indoor = false, 
                        Outdoor = true, 
                        AvailabilityStatus = "Available", 
                        LocationType = "Registered",
                        ExternalPlaceId = "ChIJaXQRs6lZwokRY6EFpJnhNNE", // Example Google Places ID
                        CreatedAt = DateTime.Now,
                        Latitude = 10.872345 ,Longitude = 106.895432
                    },
                    new Location { 
                        LocationOwnerId = owners[2].LocationOwnerId, 
                        Name = "Urban Loft", 
                        Address = "789 City Ave, New York, NY", 
                        Description = "Modern loft in city center", 
                        HourlyRate = 0, 
                        Capacity = 30, 
                        Indoor = true, 
                        Outdoor = false, 
                        AvailabilityStatus = "Busy", 
                        LocationType = "Registered",
                        ExternalPlaceId = "ChIJN1t_tDeuEmsRUsoyG83frY4", // Example Google Places ID
                        CreatedAt = DateTime.Now ,
                        Latitude = 10.823456 ,Longitude = 106.765678
                    },
                    new Location { 
                        LocationOwnerId = owners[3].LocationOwnerId, 
                        Name = "Garden Spot", 
                        Address = "321 Garden Ln, New York, NY", 
                        Description = "Lush garden for outdoor shoots", 
                        HourlyRate = 40, 
                        Capacity = 15, 
                        Indoor = false, 
                        Outdoor = true, 
                        AvailabilityStatus = "Available", 
                        LocationType = "Registered",
                        ExternalPlaceId = "ChIJKxjxuxlZwokRwA2Ire1V8mk", // Example Google Places ID
                        CreatedAt = DateTime.Now ,
                        Latitude = 10.910987 ,Longitude = 106.845321
                    },
                    new Location { 
                        LocationOwnerId = owners[4].LocationOwnerId, 
                        Name = "Mountain View", 
                        Address = "654 Hill Rd, New York, NY", 
                        Description = "Scenic mountain backdrop", 
                        HourlyRate = 80, 
                        Capacity = 25, 
                        Indoor = false, 
                        Outdoor = true, 
                        AvailabilityStatus = "Available", 
                        LocationType = "Registered",
                        ExternalPlaceId = "ChIJaXQRs6lZwokRY6EFpJnhNNE", // Example Google Places ID
                        CreatedAt = DateTime.Now ,
                        Latitude = 10.758765 ,Longitude = 106.825432
                    },
                    // 5 new locations for additional location owners
                    new Location { 
                        LocationOwnerId = owners[5].LocationOwnerId, 
                        Name = "Studio Elite", 
                        Address = "555 Elite Ave, New York, NY", 
                        Description = "Premium photography studio with state-of-the-art equipment", 
                        HourlyRate = 90, 
                        Capacity = 40, 
                        Indoor = true, 
                        Outdoor = false, 
                        AvailabilityStatus = "Available", 
                        LocationType = "Registered",
                        ExternalPlaceId = "ChIJKxjxuxlZwokRwA2Ire1V8mk", // Example Google Places ID
                        CreatedAt = DateTime.Now ,
                        Latitude = 10.935432 ,Longitude = 106.780987
                    },
                    new Location { 
                        LocationOwnerId = owners[6].LocationOwnerId, 
                        Name = "Creative Spaces", 
                        Address = "777 Creative Blvd, New York, NY", 
                        Description = "Versatile creative space for all types of photography", 
                        HourlyRate = 65, 
                        Capacity = 35, 
                        Indoor = true, 
                        Outdoor = true, 
                        AvailabilityStatus = "Available", 
                        LocationType = "Registered",
                        ExternalPlaceId = "ChIJaXQRs6lZwokRY6EFpJnhNNE", // Example Google Places ID
                        CreatedAt = DateTime.Now ,
                        Latitude = 10.801234 ,Longitude = 106.915678
                    },
                    new Location { 
                        LocationOwnerId = owners[7].LocationOwnerId, 
                        Name = "Modern Venues", 
                        Address = "888 Modern St, New York, NY", 
                        Description = "Contemporary venue with minimalist design", 
                        HourlyRate = 75, 
                        Capacity = 45, 
                        Indoor = true, 
                        Outdoor = false, 
                        AvailabilityStatus = "Available", 
                        LocationType = "Registered",
                        ExternalPlaceId = "ChIJN1t_tDeuEmsRUsoyG83frY4", // Example Google Places ID
                        CreatedAt = DateTime.Now ,
                        Latitude = 10.885678 ,Longitude = 106.735432
                    },
                    new Location { 
                        LocationOwnerId = owners[8].LocationOwnerId, 
                        Name = "Premium Studios", 
                        Address = "999 Premium Dr, New York, NY", 
                        Description = "High-end studio with luxury amenities", 
                        HourlyRate = 100, 
                        Capacity = 30, 
                        Indoor = true, 
                        Outdoor = false, 
                        AvailabilityStatus = "Available", 
                        LocationType = "Registered",
                        ExternalPlaceId = "ChIJKxjxuxlZwokRwA2Ire1V8mk", // Example Google Places ID
                        CreatedAt = DateTime.Now ,
                        Latitude = 10.769876 ,Longitude = 106.905678
                    },
                    new Location { 
                        LocationOwnerId = owners[9].LocationOwnerId, 
                        Name = "Luxury Locations", 
                        Address = "111 Luxury Way, New York, NY", 
                        Description = "Exclusive location for premium photography sessions", 
                        HourlyRate = 120, 
                        Capacity = 25, 
                        Indoor = true, 
                        Outdoor = true, 
                        AvailabilityStatus = "Available", 
                        LocationType = "Registered",
                        ExternalPlaceId = "ChIJaXQRs6lZwokRY6EFpJnhNNE", // Example Google Places ID
                        CreatedAt = DateTime.Now ,
                        Latitude = 10.942345 ,Longitude = 106.830123
                    },
                };
                context.Locations.AddRange(locations);
                context.SaveChanges();
                Console.WriteLine($"Locations seeded successfully: {locations.Length} locations added");
            }

            // Seed Bookings
            if (!context.Bookings.Any())
            {
                Console.WriteLine("Seeding Bookings...");
                var users = context.Users.Take(10).ToList();
                var photographers = context.Photographers.Take(5).ToList();
                var locations = context.Locations.Take(8).ToList(); // Now we have 8 locations (5 registered + 3 external)
                var bookings = new[]
                {
                    // Bookings with registered locations (include location fees)
                    new Booking { 
                        UserId = users[1].UserId, 
                        PhotographerId = photographers[1].PhotographerId, 
                        LocationId = locations[1].LocationId, // Riverside Venue (Registered)
                        StartDatetime = DateTime.Now.AddDays(2), 
                        EndDatetime = DateTime.Now.AddDays(2).AddHours(3), 
                        Status = "Pending", 
                        TotalPrice = 450, // Photographer (80/hr * 3) + Location (70/hr * 3) = 450
                        CreatedAt = DateTime.Now 
                    },
                    new Booking { 
                        UserId = users[2].UserId, 
                        PhotographerId = photographers[2].PhotographerId, 
                        LocationId = locations[2].LocationId, // Urban Loft (Registered)
                        StartDatetime = DateTime.Now.AddDays(3), 
                        EndDatetime = DateTime.Now.AddDays(3).AddHours(1), 
                        Status = "Confirmed", 
                        TotalPrice = 180, // Photographer (120/hr * 1) + Location (60/hr * 1) = 180
                        CreatedAt = DateTime.Now 
                    },
                    // Bookings with external locations (no location fees)
                    new Booking { 
                        UserId = users[3].UserId, 
                        PhotographerId = photographers[3].PhotographerId, 
                        LocationId = locations[5].LocationId, // Central Park (External)
                        StartDatetime = DateTime.Now.AddDays(4), 
                        EndDatetime = DateTime.Now.AddDays(4).AddHours(2), 
                        Status = "Confirmed", 
                        TotalPrice = 140, // Photographer (70/hr * 2) + Location (0/hr * 2) = 140
                        CreatedAt = DateTime.Now 
                    },
                    new Booking { 
                        UserId = users[4].UserId, 
                        PhotographerId = photographers[4].PhotographerId, 
                        LocationId = locations[6].LocationId, // Times Square (External)
                        StartDatetime = DateTime.Now.AddDays(5), 
                        EndDatetime = DateTime.Now.AddDays(5).AddHours(2), 
                        Status = "Confirmed", 
                        TotalPrice = 180, // Photographer (90/hr * 2) + Location (0/hr * 2) = 180
                        CreatedAt = DateTime.Now 
                    }
                };
                context.Bookings.AddRange(bookings);
                context.SaveChanges();
                Console.WriteLine($"Bookings seeded successfully: {bookings.Length} bookings added");
            }

            // Seed Reviews
            if (!context.Reviews.Any())
            {
                Console.WriteLine("Seeding Reviews...");
                var bookings = context.Bookings.Take(4).ToList();
                var reviews = new[]
                {
                    new Review { BookingId = bookings[0].BookingId, Rating = 5, Comment = "Excellent!", CreatedAt = DateTime.Now },
                    new Review { BookingId = bookings[1].BookingId, Rating = 4, Comment = "Very good", CreatedAt = DateTime.Now },
                    new Review { BookingId = bookings[2].BookingId, Rating = 5, Comment = "Outstanding service", CreatedAt = DateTime.Now },
                    new Review { BookingId = bookings[3].BookingId, Rating = 3, Comment = "Average", CreatedAt = DateTime.Now },
                };
                context.Reviews.AddRange(reviews);
                context.SaveChanges();
                Console.WriteLine($"Reviews seeded successfully: {reviews.Length} reviews added");
            }

            // Seed Payments
            if (!context.Payments.Any())
            {
                Console.WriteLine("Seeding Payments...");
                var bookings = context.Bookings.Take(4).ToList();
                var users = context.Users.Take(5).ToList();
                var payments = new[]
                {
                    // Payment for registered location booking (Central Park Studio)
                    new Payment { 
                        CustomerId = users[0].UserId,
                        BookingId = bookings[0].BookingId, 
                        TotalAmount = 300, 
                        Status = PaymentStatus.Success, 
                        Method = "PayOS",
                        ExternalTransactionId = "1111",
                        Note = "Payment for Central Park Studio booking",
                        CreatedAt = DateTime.Now 
                    },
                    // Payment for registered location booking (Riverside Venue)
                    new Payment { 
                        CustomerId = users[1].UserId,
                        BookingId = bookings[1].BookingId, 
                        TotalAmount = 450, 
                        Status = PaymentStatus.Pending, 
                        Method = "PayOS",
                        ExternalTransactionId = "2222",
                        Note = "Payment for Riverside Venue booking",
                        CreatedAt = DateTime.Now 
                    },
                    // Payment for registered location booking (Urban Loft)
                    new Payment { 
                        CustomerId = users[2].UserId,
                        BookingId = bookings[2].BookingId, 
                        TotalAmount = 180, 
                        Status = PaymentStatus.Success, 
                        Method = "PayOS",
                        ExternalTransactionId = "3333",
                        Note = "Payment for Urban Loft booking",
                        CreatedAt = DateTime.Now 
                    },
                    // Payment for external location booking (Central Park - no location fee)
                    new Payment { 
                        CustomerId = users[3].UserId,
                        BookingId = bookings[3].BookingId, 
                        TotalAmount = 140, 
                        Status = PaymentStatus.Success, 
                        Method = "PayOS",
                        ExternalTransactionId = "4444",
                        Note = "Payment for external location booking",
                        CreatedAt = DateTime.Now 
                    }
                };
                context.Payments.AddRange(payments);
                context.SaveChanges();
                Console.WriteLine($"Payments seeded successfully: {payments.Length} payments added");
            }

            // Seed Notifications
            if (!context.Notifications.Any())
            {
                Console.WriteLine("Seeding Notifications...");
                var users = context.Users.Take(5).ToList();
                var notifications = new[]
                {
                    new Notification { UserId = users[0].UserId, Content = "Welcome to SnapLink!", CreatedAt = DateTime.Now },
                    new Notification { UserId = users[1].UserId, Content = "Your booking is confirmed.", CreatedAt = DateTime.Now },
                    new Notification { UserId = users[2].UserId, Content = "Payment received.", CreatedAt = DateTime.Now },
                    new Notification { UserId = users[3].UserId, Content = "Review submitted.", CreatedAt = DateTime.Now },
                    new Notification { UserId = users[4].UserId, Content = "Profile updated.", CreatedAt = DateTime.Now }
                };
                context.Notifications.AddRange(notifications);
                context.SaveChanges();
                Console.WriteLine($"Notifications seeded successfully: {notifications.Length} notifications added");
            }

            // Seed Administrators
            //if (!context.Administrators.Any())
            //{
            //    var users = context.Users.Take(5).ToList();
            //    var admins = new[]
            //    {
            //        new Administrator { UserId = users[0].UserId, AccessLevel = "Super", Department = "IT" },
            //        new Administrator { UserId = users[1].UserId, AccessLevel = "Standard", Department = "Support" },
            //        new Administrator { UserId = users[2].UserId, AccessLevel = "Super", Department = "HR" },
            //        new Administrator { UserId = users[3].UserId, AccessLevel = "Standard", Department = "Finance" },
            //        new Administrator { UserId = users[4].UserId, AccessLevel = "Standard", Department = "Marketing" }
            //    };
            //    context.Administrators.AddRange(admins);
            //    context.SaveChanges();
            //}

            // Seed Moderators
            //if (!context.Moderators.Any())
            //{
            //    var users = context.Users.Take(5).ToList();
            //    var moderators = new[]
            //    {
            //        new Moderator { UserId = users[0].UserId, AreasOfFocus = "Abuse" },
            //        new Moderator { UserId = users[1].UserId, AreasOfFocus = "Spam" },
            //        new Moderator { UserId = users[2].UserId, AreasOfFocus = "Payments" },
            //        new Moderator { UserId = users[3].UserId, AreasOfFocus = "Bookings" },
            //        new Moderator { UserId = users[4].UserId, AreasOfFocus = "General" }
            //    };
            //    context.Moderators.AddRange(moderators);
            //    context.SaveChanges();
            //}

            // Seed Messagess
            //if (!context.Messagesses.Any())
            //{
            //    var users = context.Users.Take(5).ToList();
            //    var messages = new[]
            //    {
            //        new Messagess { SenderId = users[0].UserId, RecipientId = users[1].UserId, Content = "Hi Bob!", CreatedAt = DateTime.Now },
            //        new Messagess { SenderId = users[1].UserId, RecipientId = users[2].UserId, Content = "Hello Carol!", CreatedAt = DateTime.Now },
            //        new Messagess { SenderId = users[2].UserId, RecipientId = users[3].UserId, Content = "Hey Dave!", CreatedAt = DateTime.Now },
            //        new Messagess { SenderId = users[3].UserId, RecipientId = users[4].UserId, Content = "Hi Eve!", CreatedAt = DateTime.Now },
            //        new Messagess { SenderId = users[4].UserId, RecipientId = users[0].UserId, Content = "Hello Alice!", CreatedAt = DateTime.Now }
            //    };
            //    context.Messagesses.AddRange(messages);
            //    context.SaveChanges();
            //}

            // Seed PremiumPackages
            if (!context.PremiumPackages.Any())
            {
                Console.WriteLine("Seeding PremiumPackages...");
                var packages = new[]
                {
                    new PremiumPackage { Name = "Basic", Description = "Basic features", Price = 9.99M, DurationDays = 30, Features = "Standard support", ApplicableTo = "User" },
                    new PremiumPackage { Name = "Pro", Description = "Pro features", Price = 19.99M, DurationDays = 30, Features = "Priority support", ApplicableTo = "Photographer" },
                    new PremiumPackage { Name = "Elite", Description = "Elite features", Price = 29.99M, DurationDays = 90, Features = "All features", ApplicableTo = "User" },
                    new PremiumPackage { Name = "Business", Description = "Business features", Price = 49.99M, DurationDays = 180, Features = "Business tools", ApplicableTo = "Owner" },
                    new PremiumPackage { Name = "Annual", Description = "Annual plan", Price = 99.99M, DurationDays = 365, Features = "All inclusive", ApplicableTo = "All" }
                };
                context.PremiumPackages.AddRange(packages);
                context.SaveChanges();
                Console.WriteLine($"PremiumPackages seeded successfully: {packages.Length} packages added");
            }

            // Seed PremiumSubscriptions
            if (!context.PremiumSubscriptions.Any())
            {
                Console.WriteLine("Seeding PremiumSubscriptions...");
                var users = context.Users.Take(5).ToList();
                var packages = context.PremiumPackages.Take(5).ToList();
                var subscriptions = new[]
                {
                   
                    new PremiumSubscription { UserId = users[1].UserId, PackageId = packages[1].PackageId, StartDate = DateTime.Now.AddDays(-20), EndDate = DateTime.Now.AddDays(10), Status = "Expired" },
                    new PremiumSubscription { UserId = users[2].UserId, PackageId = packages[2].PackageId, StartDate = DateTime.Now.AddDays(-5), EndDate = DateTime.Now.AddDays(25), Status = "Active" },
                    new PremiumSubscription { UserId = users[3].UserId, PackageId = packages[3].PackageId, StartDate = DateTime.Now.AddDays(-15), EndDate = DateTime.Now.AddDays(15), Status = "Active" },
                    new PremiumSubscription { UserId = users[4].UserId, PackageId = packages[4].PackageId, StartDate = DateTime.Now.AddDays(-30), EndDate = DateTime.Now.AddDays(5), Status = "Expired" }
                };
                context.PremiumSubscriptions.AddRange(subscriptions);
                context.SaveChanges();
                Console.WriteLine($"PremiumSubscriptions seeded successfully: {subscriptions.Length} subscriptions added");
            }

            // Seed Transactions
            if (!context.Transactions.Any())
            {
                Console.WriteLine("Seeding Transactions...");
                var users = context.Users.Take(5).ToList();
                var transactions = new[]
                {
                 
                    new Transaction { FromUserId = users[1].UserId, ToUserId = null, Amount = 50, Type = TransactionType.Refund, Status = TransactionStatus.Success, Note = "Withdrawal", CreatedAt = DateTime.Now },
                    new Transaction { FromUserId = null, ToUserId = users[2].UserId, Amount = 75, Type = TransactionType.Refund, Status = TransactionStatus.Success, Note = "Refund for cancelled booking", CreatedAt = DateTime.Now },
                    new Transaction { FromUserId = users[3].UserId, ToUserId = null, Amount = 120, Type = TransactionType.PhotographerFee, Status = TransactionStatus.Pending, Note = "Photographer payout", CreatedAt = DateTime.Now },
                    new Transaction { FromUserId = null, ToUserId = users[4].UserId, Amount = 200, Type = TransactionType.PlatformFee, Status = TransactionStatus.Success, Note = "Referral bonus", CreatedAt = DateTime.Now }
                };
                context.Transactions.AddRange(transactions);
                context.SaveChanges();
                Console.WriteLine($"Transactions seeded successfully: {transactions.Length} transactions added");
            }

            // Seed Wallets
            if (!context.Wallets.Any())
            {
                Console.WriteLine("Seeding Wallets...");
                var users = context.Users.Take(20).ToList(); // Take all 20 users
                var wallets = new[]
                {
                    new Wallet { UserId = users[0].UserId, Balance = 0, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[1].UserId, Balance = 200000, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[2].UserId, Balance = 100000, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[3].UserId, Balance = 100000, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[4].UserId, Balance = 100000, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[5].UserId, Balance = 100000, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[6].UserId, Balance = 100000, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[7].UserId, Balance = 100000, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[8].UserId, Balance = 100000, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[9].UserId, Balance = 100000, UpdatedAt = DateTime.Now },
                    // Additional wallets for new users
                    new Wallet { UserId = users[10].UserId, Balance = 100000, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[11].UserId, Balance = 100000, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[12].UserId, Balance = 100000, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[13].UserId, Balance = 100000, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[14].UserId, Balance = 100000, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[15].UserId, Balance = 100000, UpdatedAt = DateTime.Now }, // Peter - Location Owner
                    new Wallet { UserId = users[16].UserId, Balance = 100000, UpdatedAt = DateTime.Now }, // Sarah - Location Owner
                    new Wallet { UserId = users[17].UserId, Balance = 100000, UpdatedAt = DateTime.Now }, // Mike - Location Owner
                    new Wallet { UserId = users[18].UserId, Balance = 100000, UpdatedAt = DateTime.Now }, // Lisa - Location Owner
                    new Wallet { UserId = users[19].UserId, Balance = 100000, UpdatedAt = DateTime.Now }  // David - Location Owner
                };
                context.Wallets.AddRange(wallets);
                context.SaveChanges();
                Console.WriteLine($"Wallets seeded successfully: {wallets.Length} wallets added");
            }

            // Seed WithdrawalRequests
            if (!context.WithdrawalRequests.Any())
            {
                Console.WriteLine("Seeding WithdrawalRequests...");
                var wallets = context.Wallets.Take(5).ToList();
                var requests = new[]
                {
                  
                    new WithdrawalRequest { WalletId = wallets[1].WalletId, Amount = 200, BankAccountNumber = "222222", BankAccountName = "Bob Johnson", BankName = "Bank B", RequestStatus = "Approved", RequestedAt = DateTime.Now },
                    new WithdrawalRequest { WalletId = wallets[2].WalletId, Amount = 150, BankAccountNumber = "333333", BankAccountName = "Carol White", BankName = "Bank C", RequestStatus = "Rejected", RequestedAt = DateTime.Now },
                    new WithdrawalRequest { WalletId = wallets[3].WalletId, Amount = 120, BankAccountNumber = "444444", BankAccountName = "Dave Brown", BankName = "Bank D", RequestStatus = "Pending", RequestedAt = DateTime.Now },
                    new WithdrawalRequest { WalletId = wallets[4].WalletId, Amount = 180, BankAccountNumber = "555555", BankAccountName = "Eve Black", BankName = "Bank E", RequestStatus = "Approved", RequestedAt = DateTime.Now }
                };
                context.WithdrawalRequests.AddRange(requests);
                context.SaveChanges();
                Console.WriteLine($"WithdrawalRequests seeded successfully: {requests.Length} withdrawal requests added");
            }

            // Seed Complaints
            // if (!context.Complaints.Any())
            // {
            //     Console.WriteLine("Seeding Complaints...");
            //     var users = context.Users.Take(5).ToList();
            //     var bookings = context.Bookings.Take(4).ToList();
            //     var moderators = context.Moderators.Take(5).ToList();
            //     var complaints = new[]
            //     {
            //         new Complaint { ReporterId = users[0].UserId, ReportedUserId = users[1].UserId, BookingId = bookings[0].BookingId, ComplaintType = "Service", Description = "Late arrival", Status = "Open", AssignedModeratorId = moderators[0].ModeratorId, CreatedAt = DateTime.Now },
            //         new Complaint { ReporterId = users[1].UserId, ReportedUserId = users[2].UserId, BookingId = bookings[1].BookingId, ComplaintType = "Payment", Description = "Overcharged", Status = "Closed", AssignedModeratorId = moderators[1].ModeratorId, CreatedAt = DateTime.Now },
            //         new Complaint { ReporterId = users[2].UserId, ReportedUserId = users[3].UserId, BookingId = bookings[2].BookingId, ComplaintType = "Behavior", Description = "Rude behavior", Status = "Open", AssignedModeratorId = moderators[2].ModeratorId, CreatedAt = DateTime.Now },
            //         new Complaint { ReporterId = users[3].UserId, ReportedUserId = users[4].UserId, BookingId = bookings[3].BookingId, ComplaintType = "Quality", Description = "Low quality photos", Status = "Closed", AssignedModeratorId = moderators[3].ModeratorId, CreatedAt = DateTime.Now },
            //         new Complaint { ReporterId = users[4].UserId, ReportedUserId = users[0].UserId, BookingId = bookings[4].BookingId, ComplaintType = "Other", Description = "Other issue", Status = "Open", AssignedModeratorId = moderators[4].ModeratorId, CreatedAt = DateTime.Now }
            //     };
            //     context.Complaints.AddRange(complaints);
            //     context.SaveChanges();
            //     Console.WriteLine($"Complaints seeded successfully: {complaints.Length} complaints added");
            // }

            // Seed Advertisements
            //if (!context.Advertisements.Any())
            //{
            //    Console.WriteLine("Seeding Advertisements...");
            //    var locations = context.Locations.Take(5).ToList();
            //    var payments = context.Payments.Take(5).ToList();
            //    var ads = new[]
            //    {
            //        new Advertisement { LocationId = locations[0].LocationId, Title = "Grand Opening", Description = "Special offer for new customers", ImageUrl = "url1.jpg", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30), Status = "Active", Cost = 100, PaymentId = payments[0].PaymentId },
            //        new Advertisement { LocationId = locations[1].LocationId, Title = "Summer Sale", Description = "Discounts on bookings", ImageUrl = "url2.jpg", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(15), Status = "Active", Cost = 80, PaymentId = payments[1].PaymentId },
            //        new Advertisement { LocationId = locations[2].LocationId, Title = "Wedding Season", Description = "Book now for weddings", ImageUrl = "url3.jpg", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(45), Status = "Inactive", Cost = 120, PaymentId = payments[2].PaymentId },
            //        new Advertisement { LocationId = locations[3].LocationId, Title = "Photo Contest", Description = "Join and win prizes", ImageUrl = "url4.jpg", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(10), Status = "Active", Cost = 60, PaymentId = payments[3].PaymentId },
            //        new Advertisement { LocationId = locations[4].LocationId, Title = "Holiday Shoots", Description = "Special holiday packages", ImageUrl = "url5.jpg", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(20), Status = "Active", Cost = 90, PaymentId = payments[4].PaymentId }
            //    };
            //    context.Advertisements.AddRange(ads);
            //    context.SaveChanges();
            //    Console.WriteLine($"Advertisements seeded successfully: {ads.Length} advertisements added");
            //}

            // Seed PhotographerEvents
            // Seed Images
            if (!context.Images.Any())
            {
                Console.WriteLine("Seeding Images...");
                var photographers = context.Photographers.Take(5).ToList();
                var locations = context.Locations.Take(8).ToList();
                var images = new[]
                {
                    //photographer 1
                    new Image { Url = "https://snaplinkstorage.blob.core.windows.net/snaplinkstorageblob/photographer/1/linggka109_3693931479659561498's2025-8-11-11.11.433%20story.jpg", PhotographerId = photographers[0].PhotographerId, IsPrimary = false, Caption = null,Status =ImageStatus.Safe, CreatedAt = DateTime.Parse("2025-07-15 03:10:01.043") },
                    new Image { Url = "https://snaplinkstorage.blob.core.windows.net/snaplinkstorageblob/photographer/1/linggka109_3693931479626133367's2025-8-11-11.12.861%20story.jpg", PhotographerId = photographers[0].PhotographerId, IsPrimary = false, Caption = null,Status =ImageStatus.Safe, CreatedAt = DateTime.Parse("2025-07-15 03:10:01.043") },
                    new Image { Url = "https://snaplinkstorage.blob.core.windows.net/snaplinkstorageblob/photographer/1/linggka109_3693931479491924526's2025-8-11-11.12.433%20story.jpg", PhotographerId = photographers[0].PhotographerId, IsPrimary = false, Caption = null,Status =ImageStatus.Safe, CreatedAt = DateTime.Parse("2025-07-15 03:10:01.043") },
                    new Image { Url = "https://snaplinkstorage.blob.core.windows.net/snaplinkstorageblob/photographer/1/linggka109_3693931479491869661's2025-8-11-11.12.171%20story.jpg", PhotographerId = photographers[0].PhotographerId, IsPrimary = false, Caption = null,Status =ImageStatus.Safe, CreatedAt = DateTime.Parse("2025-07-15 03:10:01.043") },
                    new Image { Url = "https://snaplinkstorage.blob.core.windows.net/snaplinkstorageblob/photographer/1/linggka109_3693931479483441064's2025-8-11-11.12.953%20story.jpg", PhotographerId = photographers[0].PhotographerId, IsPrimary = false, Caption = null,Status =ImageStatus.Safe, CreatedAt = DateTime.Parse("2025-07-15 03:10:01.043") },
                    new Image { Url = "https://snaplinkstorage.blob.core.windows.net/snaplinkstorageblob/photographer/1/linggka109_3693931479634527198's2025-8-11-11.14.746%20story.jpg", PhotographerId = photographers[0].PhotographerId, IsPrimary = false, Caption = null,Status =ImageStatus.Safe, CreatedAt = DateTime.Parse("2025-07-15 03:10:01.043") },
                    //photographer 2
                    new Image { Url = "https://snaplinkstorage.blob.core.windows.net/snaplinkstorageblob/photographer/2/linggka109_2864048423449716041.png", PhotographerId = photographers[1].PhotographerId, IsPrimary = false, Caption = null,Status =ImageStatus.Safe, CreatedAt = DateTime.Parse("2025-07-15 03:10:01.043") },
                    new Image { Url = "https://snaplinkstorage.blob.core.windows.net/snaplinkstorageblob/photographer/2/linggka109_2864048423458271940.png", PhotographerId = photographers[1].PhotographerId, IsPrimary = false, Caption = null,Status =ImageStatus.Safe, CreatedAt = DateTime.Parse("2025-07-15 03:13:35.237") },
                    new Image { Url = "https://snaplinkstorage.blob.core.windows.net/snaplinkstorageblob/photographer/2/linggka109_2864048423575690541.png", PhotographerId = photographers[1].PhotographerId, IsPrimary = false, Caption = null,Status =ImageStatus.Safe, CreatedAt = DateTime.Parse("2025-07-15 05:26:45.567") },
                    new Image { Url = "https://snaplinkstorage.blob.core.windows.net/snaplinkstorageblob/photographer/2/linggka109_2864048423583882606.png", PhotographerId = photographers[1].PhotographerId, IsPrimary = false, Caption = null,Status =ImageStatus.Safe, CreatedAt = DateTime.Parse("2025-07-15 05:39:02.590") },
                    new Image { Url = "https://snaplinkstorage.blob.core.windows.net/snaplinkstorageblob/photographer/2/linggka109_2864048423584006215.png", PhotographerId = photographers[1].PhotographerId, IsPrimary = false, Caption = null,Status =ImageStatus.Safe, CreatedAt = DateTime.Parse("2025-07-15 05:39:02.590") },
                    new Image { Url = "https://snaplinkstorage.blob.core.windows.net/snaplinkstorageblob/photographer/2/linggka109_2864048423609250785.png", PhotographerId = photographers[1].PhotographerId, IsPrimary = false, Caption = null,Status =ImageStatus.Safe, CreatedAt = DateTime.Parse("2025-07-15 05:39:02.590") },
                    //location 
                    new Image { Url = "https://snaplinkstorage.blob.core.windows.net/snaplinkstorageblob/location/1/118493381_725239154721936_7128225189019907505_n.jpg", LocationId = locations[0].LocationId, IsPrimary = true, Caption = "Location 1",Status =ImageStatus.Safe, CreatedAt = DateTime.Now },
                    new Image { Url = "https://picsum.photos/200/300?random=18", LocationId = locations[0].LocationId, IsPrimary = true, Caption = "Location 1",Status =ImageStatus.Safe, CreatedAt = DateTime.Now },
                    new Image { Url = "https://picsum.photos/200/300?random=19", LocationId = locations[0].LocationId, IsPrimary = true, Caption = "Location 1",Status =ImageStatus.Safe, CreatedAt = DateTime.Now },
                    new Image { Url = "https://picsum.photos/200/300?random=20", LocationId = locations[0].LocationId, IsPrimary = true, Caption = "Location 1",Status =ImageStatus.Safe, CreatedAt = DateTime.Now },
                    new Image { Url = "https://snaplinkstorage.blob.core.windows.net/snaplinkstorageblob/location/2/Halleyparknovember_b.jpg", LocationId = locations[1].LocationId, IsPrimary = true, Caption = "Location 2",Status =ImageStatus.Safe, CreatedAt = DateTime.Now },
                    new Image { Url = "https://snaplinkstorage.blob.core.windows.net/snaplinkstorageblob/location/3/seafood-buffet-at-mezz.jpg", LocationId = locations[2].LocationId, IsPrimary = true, Caption = "Location 3",Status =ImageStatus.Safe, CreatedAt = DateTime.Now },
                    new Image { Url = "https://picsum.photos/200/300?random=21", LocationId = locations[3].LocationId, IsPrimary = true, Caption = "Location 4",Status =ImageStatus.Safe, CreatedAt = DateTime.Now },
                    new Image { Url = "https://picsum.photos/200/300?random=22", LocationId = locations[4].LocationId, IsPrimary = true, Caption = "Location 5", Status = ImageStatus.Safe, CreatedAt = DateTime.Now },
                    new Image { Url = "https://picsum.photos/200/300?random=23", LocationId = locations[5].LocationId, IsPrimary = true, Caption = "Location 6",Status =ImageStatus.Safe, CreatedAt = DateTime.Now },
                    new Image { Url = "https://picsum.photos/200/300?random=24", LocationId = locations[6].LocationId, IsPrimary = true, Caption = "Location 7",Status =ImageStatus.Safe, CreatedAt = DateTime.Now },
                    new Image { Url = "https://picsum.photos/200/300?random=25", LocationId = locations[7].LocationId, IsPrimary = true, Caption = "Location 8",Status =ImageStatus.Safe, CreatedAt = DateTime.Now }
                };
                context.Images.AddRange(images);
                context.SaveChanges();
                Console.WriteLine($"Images seeded successfully: {images.Length} images added");
            }

            // Seed UserStyles
            if (!context.UserStyles.Any())
            {
                Console.WriteLine("Seeding UserStyles...");
                var users = context.Users.Take(5).ToList();
                var styles = context.Styles.Take(5).ToList();
                var userStyles = new[]
                {
                    new UserStyle { UserId = users[0].UserId, StyleId = styles[0].StyleId, CreatedAt = DateTime.Now },
                    new UserStyle { UserId = users[1].UserId, StyleId = styles[1].StyleId, CreatedAt = DateTime.Now },
                    new UserStyle { UserId = users[2].UserId, StyleId = styles[2].StyleId, CreatedAt = DateTime.Now },
                    new UserStyle { UserId = users[3].UserId, StyleId = styles[3].StyleId, CreatedAt = DateTime.Now },
                    new UserStyle { UserId = users[4].UserId, StyleId = styles[4].StyleId, CreatedAt = DateTime.Now }
                };
                context.UserStyles.AddRange(userStyles);
                context.SaveChanges();
                Console.WriteLine($"UserStyles seeded successfully: {userStyles.Length} user styles added");
            }

            // Seed Availability
            if (!context.Availabilities.Any())
            {
                Console.WriteLine("Seeding Availabilities...");
                var photographers = context.Photographers.Take(15).ToList();
                var availabilities = new List<Availability>();

                // Photographer 1 (Alice) - Portrait specialist - Available Monday to Friday
                for (int i = 1; i <= 5; i++) // Monday to Friday
                {
                    availabilities.Add(new Availability
                    {
                        PhotographerId = photographers[0].PhotographerId,
                        DayOfWeek = (DayOfWeek)i,
                        StartTime = new TimeSpan(9, 0, 0), // 9:00 AM
                        EndTime = new TimeSpan(17, 0, 0),  // 5:00 PM
                        Status = "Available",
                        CreatedAt = DateTime.Now
                    });
                }

                // Photographer 2 (Bob) - Landscape specialist - Available weekends and evenings
                availabilities.Add(new Availability
                {
                    PhotographerId = photographers[1].PhotographerId,
                    DayOfWeek = DayOfWeek.Saturday,
                    StartTime = new TimeSpan(6, 0, 0), // 6:00 AM for sunrise
                    EndTime = new TimeSpan(18, 0, 0),  // 6:00 PM
                    Status = "Available",
                    CreatedAt = DateTime.Now
                });
                availabilities.Add(new Availability
                {
                    PhotographerId = photographers[1].PhotographerId,
                    DayOfWeek = DayOfWeek.Sunday,
                    StartTime = new TimeSpan(6, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    Status = "Available",
                    CreatedAt = DateTime.Now
                });
                // Weekday evenings
                for (int i = 1; i <= 5; i++)
                {
                    availabilities.Add(new Availability
                    {
                        PhotographerId = photographers[1].PhotographerId,
                        DayOfWeek = (DayOfWeek)i,
                        StartTime = new TimeSpan(18, 0, 0), // 6:00 PM
                        EndTime = new TimeSpan(22, 0, 0),   // 10:00 PM
                        Status = "Available",
                        CreatedAt = DateTime.Now
                    });
                }

                // Photographer 3 (Carol) - Wedding specialist - Available all week, flexible hours
                for (int i = 1; i <= 7; i++) // All days
                {
                    availabilities.Add(new Availability
                    {
                        PhotographerId = photographers[2].PhotographerId,
                        DayOfWeek = (DayOfWeek)i,
                        StartTime = new TimeSpan(8, 0, 0), // 8:00 AM
                        EndTime = new TimeSpan(20, 0, 0),  // 8:00 PM
                        Status = "Available",
                        CreatedAt = DateTime.Now
                    });
                }

                // Photographer 4 (Dave) - Event specialist - Available weekdays only
                for (int i = 1; i <= 5; i++)
                {
                    availabilities.Add(new Availability
                    {
                        PhotographerId = photographers[3].PhotographerId,
                        DayOfWeek = (DayOfWeek)i,
                        StartTime = new TimeSpan(10, 0, 0), // 10:00 AM
                        EndTime = new TimeSpan(18, 0, 0),   // 6:00 PM
                        Status = "Available",
                        CreatedAt = DateTime.Now
                    });
                }

                // Photographer 5 (Eve) - Fashion specialist - Available Tuesday to Saturday
                for (int i = 2; i <= 6; i++) // Tuesday to Saturday
                {
                    availabilities.Add(new Availability
                    {
                        PhotographerId = photographers[4].PhotographerId,
                        DayOfWeek = (DayOfWeek)i,
                        StartTime = new TimeSpan(11, 0, 0), // 11:00 AM
                        EndTime = new TimeSpan(19, 0, 0),   // 7:00 PM
                        Status = "Available",
                        CreatedAt = DateTime.Now
                    });
                }

                // Photographer 6 (Frank) - Portrait + Fashion - Available Monday to Friday, morning and evening
                for (int i = 1; i <= 5; i++)
                {
                    // Morning session
                    availabilities.Add(new Availability
                    {
                        PhotographerId = photographers[5].PhotographerId,
                        DayOfWeek = (DayOfWeek)i,
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(12, 0, 0),
                        Status = "Available",
                        CreatedAt = DateTime.Now
                    });
                    // Afternoon session
                    availabilities.Add(new Availability
                    {
                        PhotographerId = photographers[5].PhotographerId,
                        DayOfWeek = (DayOfWeek)i,
                        StartTime = new TimeSpan(14, 0, 0), // 2:00 PM
                        EndTime = new TimeSpan(18, 0, 0),   // 6:00 PM
                        Status = "Available",
                        CreatedAt = DateTime.Now
                    });
                }

                // Photographer 7 (Grace) - Landscape + Event - Available weekends and weekday evenings
                // Weekends
                availabilities.Add(new Availability
                {
                    PhotographerId = photographers[6].PhotographerId,
                    DayOfWeek = DayOfWeek.Saturday,
                    StartTime = new TimeSpan(7, 0, 0),
                    EndTime = new TimeSpan(19, 0, 0),
                    Status = "Available",
                    CreatedAt = DateTime.Now
                });
                availabilities.Add(new Availability
                {
                    PhotographerId = photographers[6].PhotographerId,
                    DayOfWeek = DayOfWeek.Sunday,
                    StartTime = new TimeSpan(7, 0, 0),
                    EndTime = new TimeSpan(19, 0, 0),
                    Status = "Available",
                    CreatedAt = DateTime.Now
                });
                // Weekday evenings
                for (int i = 1; i <= 5; i++)
                {
                    availabilities.Add(new Availability
                    {
                        PhotographerId = photographers[6].PhotographerId,
                        DayOfWeek = (DayOfWeek)i,
                        StartTime = new TimeSpan(17, 0, 0), // 5:00 PM
                        EndTime = new TimeSpan(21, 0, 0),   // 9:00 PM
                        Status = "Available",
                        CreatedAt = DateTime.Now
                    });
                }

                // Photographer 8 (Henry) - Wedding + Portrait + Event - Available all week, long hours
                for (int i = 1; i <= 7; i++)
                {
                    availabilities.Add(new Availability
                    {
                        PhotographerId = photographers[7].PhotographerId,
                        DayOfWeek = (DayOfWeek)i,
                        StartTime = new TimeSpan(7, 0, 0), // 7:00 AM
                        EndTime = new TimeSpan(22, 0, 0),  // 10:00 PM
                        Status = "Available",
                        CreatedAt = DateTime.Now
                    });
                }

                // Photographer 9 (Iris) - Event specialist - Available weekdays and Saturday
                for (int i = 1; i <= 6; i++) // Monday to Saturday
                {
                    availabilities.Add(new Availability
                    {
                        PhotographerId = photographers[8].PhotographerId,
                        DayOfWeek = (DayOfWeek)i,
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(17, 0, 0),
                        Status = "Available",
                        CreatedAt = DateTime.Now
                    });
                }

                // Photographer 10 (Jack) - Fashion + Portrait - Available Tuesday to Sunday
                for (int i = 2; i <= 7; i++) // Tuesday to Sunday
                {
                    availabilities.Add(new Availability
                    {
                        PhotographerId = photographers[9].PhotographerId,
                        DayOfWeek = (DayOfWeek)i,
                        StartTime = new TimeSpan(10, 0, 0),
                        EndTime = new TimeSpan(18, 0, 0),
                        Status = "Available",
                        CreatedAt = DateTime.Now
                    });
                }

                // Photographer 11 (Kate) - Portrait + Wedding - Available Monday to Friday
                for (int i = 1; i <= 5; i++)
                {
                    availabilities.Add(new Availability
                    {
                        PhotographerId = photographers[10].PhotographerId,
                        DayOfWeek = (DayOfWeek)i,
                        StartTime = new TimeSpan(8, 0, 0),
                        EndTime = new TimeSpan(16, 0, 0),
                        Status = "Available",
                        CreatedAt = DateTime.Now
                    });
                }

                // Photographer 12 (Leo) - Landscape specialist - Available weekends and weekday mornings
                // Weekends
                availabilities.Add(new Availability
                {
                    PhotographerId = photographers[11].PhotographerId,
                    DayOfWeek = DayOfWeek.Saturday,
                    StartTime = new TimeSpan(5, 0, 0), // 5:00 AM for sunrise
                    EndTime = new TimeSpan(17, 0, 0),
                    Status = "Available",
                    CreatedAt = DateTime.Now
                });
                availabilities.Add(new Availability
                {
                    PhotographerId = photographers[11].PhotographerId,
                    DayOfWeek = DayOfWeek.Sunday,
                    StartTime = new TimeSpan(5, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    Status = "Available",
                    CreatedAt = DateTime.Now
                });
                // Weekday mornings
                for (int i = 1; i <= 5; i++)
                {
                    availabilities.Add(new Availability
                    {
                        PhotographerId = photographers[11].PhotographerId,
                        DayOfWeek = (DayOfWeek)i,
                        StartTime = new TimeSpan(6, 0, 0), // 6:00 AM
                        EndTime = new TimeSpan(12, 0, 0),  // 12:00 PM
                        Status = "Available",
                        CreatedAt = DateTime.Now
                    });
                }

                // Photographer 13 (Maya) - Wedding + Fashion + Event - Available all week
                for (int i = 1; i <= 7; i++)
                {
                    availabilities.Add(new Availability
                    {
                        PhotographerId = photographers[12].PhotographerId,
                        DayOfWeek = (DayOfWeek)i,
                        StartTime = new TimeSpan(8, 0, 0),
                        EndTime = new TimeSpan(20, 0, 0),
                        Status = "Available",
                        CreatedAt = DateTime.Now
                    });
                }

                // Photographer 14 (Nina) - Event + Landscape - Available weekdays and weekend afternoons
                // Weekdays
                for (int i = 1; i <= 5; i++)
                {
                    availabilities.Add(new Availability
                    {
                        PhotographerId = photographers[13].PhotographerId,
                        DayOfWeek = (DayOfWeek)i,
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(17, 0, 0),
                        Status = "Available",
                        CreatedAt = DateTime.Now
                    });
                }
                // Weekend afternoons
                availabilities.Add(new Availability
                {
                    PhotographerId = photographers[13].PhotographerId,
                    DayOfWeek = DayOfWeek.Saturday,
                    StartTime = new TimeSpan(13, 0, 0), // 1:00 PM
                    EndTime = new TimeSpan(19, 0, 0),   // 7:00 PM
                    Status = "Available",
                    CreatedAt = DateTime.Now
                });
                availabilities.Add(new Availability
                {
                    PhotographerId = photographers[13].PhotographerId,
                    DayOfWeek = DayOfWeek.Sunday,
                    StartTime = new TimeSpan(13, 0, 0),
                    EndTime = new TimeSpan(19, 0, 0),
                    Status = "Available",
                    CreatedAt = DateTime.Now
                });

                // Photographer 15 (Oscar) - Fashion + Portrait + Wedding - Available Tuesday to Sunday
                for (int i = 2; i <= 7; i++) // Tuesday to Sunday
                {
                    availabilities.Add(new Availability
                    {
                        PhotographerId = photographers[14].PhotographerId,
                        DayOfWeek = (DayOfWeek)i,
                        StartTime = new TimeSpan(10, 0, 0),
                        EndTime = new TimeSpan(19, 0, 0),
                        Status = "Available",
                        CreatedAt = DateTime.Now
                    });
                }

                context.Availabilities.AddRange(availabilities);
                context.SaveChanges();
                Console.WriteLine($"Availabilities seeded successfully: {availabilities.Count} availabilities added");
            }

            // Seed DeviceInfo
            if (!context.DeviceInfos.Any())
            {
                Console.WriteLine("Seeding DeviceInfos...");
                var photographers = context.Photographers.Take(15).ToList();
                var deviceInfos = new List<DeviceInfo>();

                // Photographer 1 (Alice) - iPhone 15 Pro
                deviceInfos.Add(new DeviceInfo
                {
                    PhotographerId = photographers[0].PhotographerId,
                    DeviceType = "Phone",
                    Brand = "Apple",
                    Model = "iPhone 15 Pro",
                    OperatingSystem = "iOS",
                    OsVersion = "17.2.1",
                    ScreenResolution = "2556x1179",
                    CameraResolution = "48MP",
                    StorageCapacity = "256GB",
                    BatteryCapacity = "4441mAh",
                    Features = "{\"NightMode\": true, \"PortraitMode\": true, \"4KVideo\": true, \"ProRAW\": true, \"CinematicMode\": true, \"ActionMode\": true}",
                    Status = "Active",
                    Notes = "Primary device for mobile photography",
                    LastUsedAt = DateTime.Now.AddDays(-2),
                    CreatedAt = DateTime.Now.AddDays(-30)
                });

                // Photographer 2 (Bob) - Samsung Galaxy S24 Ultra
                deviceInfos.Add(new DeviceInfo
                {
                    PhotographerId = photographers[1].PhotographerId,
                    DeviceType = "Phone",
                    Brand = "Samsung",
                    Model = "Galaxy S24 Ultra",
                    OperatingSystem = "Android",
                    OsVersion = "14.0",
                    ScreenResolution = "3088x1440",
                    CameraResolution = "200MP",
                    StorageCapacity = "512GB",
                    BatteryCapacity = "5000mAh",
                    Features = "{\"NightMode\": true, \"PortraitMode\": true, \"8KVideo\": true, \"SpaceZoom\": true, \"S_Pen\": true, \"AI_Features\": true}",
                    Status = "Active",
                    Notes = "Professional mobile photography with S Pen",
                    LastUsedAt = DateTime.Now.AddDays(-1),
                    CreatedAt = DateTime.Now.AddDays(-25)
                });

                // Photographer 3 (Carol) - iPhone 15 Pro Max
                deviceInfos.Add(new DeviceInfo
                {
                    PhotographerId = photographers[2].PhotographerId,
                    DeviceType = "Phone",
                    Brand = "Apple",
                    Model = "iPhone 15 Pro Max",
                    OperatingSystem = "iOS",
                    OsVersion = "17.2.1",
                    ScreenResolution = "2796x1290",
                    CameraResolution = "48MP",
                    StorageCapacity = "512GB",
                    BatteryCapacity = "4441mAh",
                    Features = "{\"NightMode\": true, \"PortraitMode\": true, \"4KVideo\": true, \"ProRAW\": true, \"CinematicMode\": true, \"ActionMode\": true, \"5xOpticalZoom\": true}",
                    Status = "Active",
                    Notes = "Premium device with 5x optical zoom for versatile photography",
                    LastUsedAt = DateTime.Now.AddDays(-3),
                    CreatedAt = DateTime.Now.AddDays(-40)
                });

                // Photographer 4 (Dave) - iPhone 14 Pro
                deviceInfos.Add(new DeviceInfo
                {
                    PhotographerId = photographers[3].PhotographerId,
                    DeviceType = "Phone",
                    Brand = "Apple",
                    Model = "iPhone 14 Pro",
                    OperatingSystem = "iOS",
                    OsVersion = "17.1.2",
                    ScreenResolution = "2556x1179",
                    CameraResolution = "48MP",
                    StorageCapacity = "128GB",
                    BatteryCapacity = "3200mAh",
                    Features = "{\"NightMode\": true, \"PortraitMode\": true, \"4KVideo\": true, \"ProRAW\": true, \"CinematicMode\": true}",
                    Status = "Active",
                    Notes = "Reliable device for event photography",
                    LastUsedAt = DateTime.Now.AddDays(-5),
                    CreatedAt = DateTime.Now.AddDays(-35)
                });

                // Photographer 5 (Eve) - Google Pixel 8 Pro
                deviceInfos.Add(new DeviceInfo
                {
                    PhotographerId = photographers[4].PhotographerId,
                    DeviceType = "Phone",
                    Brand = "Google",
                    Model = "Pixel 8 Pro",
                    OperatingSystem = "Android",
                    OsVersion = "14.0",
                    ScreenResolution = "2992x1344",
                    CameraResolution = "50MP",
                    StorageCapacity = "256GB",
                    BatteryCapacity = "4950mAh",
                    Features = "{\"NightMode\": true, \"PortraitMode\": true, \"4KVideo\": true, \"MagicEraser\": true, \"BestTake\": true, \"AI_Features\": true}",
                    Status = "Active",
                    Notes = "AI-powered photography with Google's computational photography",
                    LastUsedAt = DateTime.Now.AddDays(-1),
                    CreatedAt = DateTime.Now.AddDays(-45)
                });

                // Photographer 6 (Frank) - Samsung Galaxy S23 Ultra
                deviceInfos.Add(new DeviceInfo
                {
                    PhotographerId = photographers[5].PhotographerId,
                    DeviceType = "Phone",
                    Brand = "Samsung",
                    Model = "Galaxy S23 Ultra",
                    OperatingSystem = "Android",
                    OsVersion = "13.0",
                    ScreenResolution = "3088x1440",
                    CameraResolution = "200MP",
                    StorageCapacity = "256GB",
                    BatteryCapacity = "5000mAh",
                    Features = "{\"NightMode\": true, \"PortraitMode\": true, \"8KVideo\": true, \"SpaceZoom\": true, \"S_Pen\": true}",
                    Status = "Active",
                    Notes = "Previous generation flagship with excellent camera capabilities",
                    LastUsedAt = DateTime.Now.AddDays(-2),
                    CreatedAt = DateTime.Now.AddDays(-20)
                });

                // Photographer 7 (Grace) - iPhone 14 Pro Max
                deviceInfos.Add(new DeviceInfo
                {
                    PhotographerId = photographers[6].PhotographerId,
                    DeviceType = "Phone",
                    Brand = "Apple",
                    Model = "iPhone 14 Pro Max",
                    OperatingSystem = "iOS",
                    OsVersion = "17.1.2",
                    ScreenResolution = "2796x1290",
                    CameraResolution = "48MP",
                    StorageCapacity = "512GB",
                    BatteryCapacity = "4323mAh",
                    Features = "{\"NightMode\": true, \"PortraitMode\": true, \"4KVideo\": true, \"ProRAW\": true, \"CinematicMode\": true, \"ActionMode\": true}",
                    Status = "Active",
                    Notes = "Large screen device for professional mobile photography",
                    LastUsedAt = DateTime.Now.AddDays(-4),
                    CreatedAt = DateTime.Now.AddDays(-50)
                });

                // Photographer 8 (Henry) - Samsung Galaxy S24+
                deviceInfos.Add(new DeviceInfo
                {
                    PhotographerId = photographers[7].PhotographerId,
                    DeviceType = "Phone",
                    Brand = "Samsung",
                    Model = "Galaxy S24+",
                    OperatingSystem = "Android",
                    OsVersion = "14.0",
                    ScreenResolution = "3088x1440",
                    CameraResolution = "50MP",
                    StorageCapacity = "512GB",
                    BatteryCapacity = "4900mAh",
                    Features = "{\"NightMode\": true, \"PortraitMode\": true, \"8KVideo\": true, \"SpaceZoom\": true, \"AI_Features\": true}",
                    Status = "Active",
                    Notes = "Latest Samsung flagship with advanced AI features",
                    LastUsedAt = DateTime.Now.AddDays(-1),
                    CreatedAt = DateTime.Now.AddDays(-15)
                });

                // Photographer 9 (Iris) - Google Pixel 7 Pro
                deviceInfos.Add(new DeviceInfo
                {
                    PhotographerId = photographers[8].PhotographerId,
                    DeviceType = "Phone",
                    Brand = "Google",
                    Model = "Pixel 7 Pro",
                    OperatingSystem = "Android",
                    OsVersion = "14.0",
                    ScreenResolution = "3120x1440",
                    CameraResolution = "50MP",
                    StorageCapacity = "128GB",
                    BatteryCapacity = "5000mAh",
                    Features = "{\"NightMode\": true, \"PortraitMode\": true, \"4KVideo\": true, \"MagicEraser\": true, \"BestTake\": true}",
                    Status = "Active",
                    Notes = "Previous generation Pixel with excellent computational photography",
                    LastUsedAt = DateTime.Now.AddDays(-3),
                    CreatedAt = DateTime.Now.AddDays(-30)
                });

                // Photographer 10 (Jack) - iPhone 13 Pro Max
                deviceInfos.Add(new DeviceInfo
                {
                    PhotographerId = photographers[9].PhotographerId,
                    DeviceType = "Phone",
                    Brand = "Apple",
                    Model = "iPhone 13 Pro Max",
                    OperatingSystem = "iOS",
                    OsVersion = "17.1.2",
                    ScreenResolution = "2778x1284",
                    CameraResolution = "12MP",
                    StorageCapacity = "256GB",
                    BatteryCapacity = "4352mAh",
                    Features = "{\"NightMode\": true, \"PortraitMode\": true, \"4KVideo\": true, \"ProRAW\": true, \"CinematicMode\": true}",
                    Status = "Active",
                    Notes = "Reliable device for everyday photography",
                    LastUsedAt = DateTime.Now.AddDays(-6),
                    CreatedAt = DateTime.Now.AddDays(-60)
                });

                // Photographer 11 (Kate) - Samsung Galaxy A54
                deviceInfos.Add(new DeviceInfo
                {
                    PhotographerId = photographers[10].PhotographerId,
                    DeviceType = "Phone",
                    Brand = "Samsung",
                    Model = "Galaxy A54",
                    OperatingSystem = "Android",
                    OsVersion = "14.0",
                    ScreenResolution = "2400x1080",
                    CameraResolution = "50MP",
                    StorageCapacity = "256GB",
                    BatteryCapacity = "5000mAh",
                    Features = "{\"NightMode\": true, \"PortraitMode\": true, \"4KVideo\": true, \"OIS\": true}",
                    Status = "Active",
                    Notes = "Mid-range device with excellent camera capabilities",
                    LastUsedAt = DateTime.Now.AddDays(-2),
                    CreatedAt = DateTime.Now.AddDays(-35)
                });

                // Photographer 12 (Leo) - iPhone 12 Pro
                deviceInfos.Add(new DeviceInfo
                {
                    PhotographerId = photographers[11].PhotographerId,
                    DeviceType = "Phone",
                    Brand = "Apple",
                    Model = "iPhone 12 Pro",
                    OperatingSystem = "iOS",
                    OsVersion = "17.1.2",
                    ScreenResolution = "2532x1170",
                    CameraResolution = "12MP",
                    StorageCapacity = "256GB",
                    BatteryCapacity = "2815mAh",
                    Features = "{\"NightMode\": true, \"PortraitMode\": true, \"4KVideo\": true, \"ProRAW\": true}",
                    Status = "Active",
                    Notes = "Reliable device for everyday photography",
                    LastUsedAt = DateTime.Now.AddDays(-7),
                    CreatedAt = DateTime.Now.AddDays(-80)
                });

                // Photographer 13 (Maya) - Google Pixel 6 Pro
                deviceInfos.Add(new DeviceInfo
                {
                    PhotographerId = photographers[12].PhotographerId,
                    DeviceType = "Phone",
                    Brand = "Google",
                    Model = "Pixel 6 Pro",
                    OperatingSystem = "Android",
                    OsVersion = "14.0",
                    ScreenResolution = "3120x1440",
                    CameraResolution = "50MP",
                    StorageCapacity = "256GB",
                    BatteryCapacity = "5003mAh",
                    Features = "{\"NightMode\": true, \"PortraitMode\": true, \"4KVideo\": true, \"MagicEraser\": true}",
                    Status = "Active",
                    Notes = "Previous generation Pixel with excellent camera",
                    LastUsedAt = DateTime.Now.AddDays(-1),
                    CreatedAt = DateTime.Now.AddDays(-40)
                });

                // Photographer 14 (Nina) - Samsung Galaxy S22 Ultra
                deviceInfos.Add(new DeviceInfo
                {
                    PhotographerId = photographers[13].PhotographerId,
                    DeviceType = "Phone",
                    Brand = "Samsung",
                    Model = "Galaxy S22 Ultra",
                    OperatingSystem = "Android",
                    OsVersion = "14.0",
                    ScreenResolution = "3088x1440",
                    CameraResolution = "108MP",
                    StorageCapacity = "256GB",
                    BatteryCapacity = "5000mAh",
                    Features = "{\"NightMode\": true, \"PortraitMode\": true, \"8KVideo\": true, \"SpaceZoom\": true, \"S_Pen\": true}",
                    Status = "Active",
                    Notes = "Previous generation Ultra with S Pen",
                    LastUsedAt = DateTime.Now.AddDays(-4),
                    CreatedAt = DateTime.Now.AddDays(-70)
                });

                // Photographer 15 (Oscar) - iPhone 11 Pro Max
                deviceInfos.Add(new DeviceInfo
                {
                    PhotographerId = photographers[14].PhotographerId,
                    DeviceType = "Phone",
                    Brand = "Apple",
                    Model = "iPhone 11 Pro Max",
                    OperatingSystem = "iOS",
                    OsVersion = "17.1.2",
                    ScreenResolution = "2688x1242",
                    CameraResolution = "12MP",
                    StorageCapacity = "256GB",
                    BatteryCapacity = "3969mAh",
                    Features = "{\"NightMode\": true, \"PortraitMode\": true, \"4KVideo\": true}",
                    Status = "Active",
                    Notes = "Older but still capable device",
                    LastUsedAt = DateTime.Now.AddDays(-3),
                    CreatedAt = DateTime.Now.AddDays(-55)
                });
                context.DeviceInfos.AddRange(deviceInfos);
                context.SaveChanges();
                Console.WriteLine($"DeviceInfos seeded successfully: {deviceInfos.Count} device infos added");
            }

            // Seed PhotoDeliveries
            if (!context.PhotoDeliveries.Any())
            {
                Console.WriteLine("Seeding PhotoDeliveries...");
                var bookings = context.Bookings.Take(4).ToList();
                var photoDeliveries = new[]
                {
                    // PhotoDelivery 1 - Completed delivery via Google Drive
                    new PhotoDelivery { 
                        BookingId = bookings[0].BookingId, 
                        DeliveryMethod = "PhotographerDevice", 
                        DriveLink = "https://drive.google.com/drive/folders/1ABC123DEF456GHI789JKL", 
                        DriveFolderName = "Alice_Smith_Portrait_Session_2024", 
                        PhotoCount = 45, 
                        Status = "Delivered", 
                        UploadedAt = DateTime.Now.AddDays(-2), 
                        DeliveredAt = DateTime.Now.AddDays(-1), 
                        ExpiresAt = DateTime.Now.AddDays(30), 
                        Notes = "Portrait session photos - 45 high-quality images delivered", 
                        CreatedAt = DateTime.Now.AddDays(-3), 
                        UpdatedAt = DateTime.Now.AddDays(-1) 
                    },
                    
                    // PhotoDelivery 2 - Pending delivery
                    new PhotoDelivery { 
                        BookingId = bookings[1].BookingId, 
                        DeliveryMethod = "CustomerDevice", 
                        DriveLink = null, 
                        DriveFolderName = null, 
                        PhotoCount = null, 
                        Status = "Pending", 
                        UploadedAt = null, 
                        DeliveredAt = null, 
                        ExpiresAt = null, 
                        Notes = "Customer will provide device for direct transfer", 
                        CreatedAt = DateTime.Now.AddDays(-1), 
                        UpdatedAt = DateTime.Now.AddDays(-1) 
                    },
                    
                    // PhotoDelivery 3 - Currently uploading
                    new PhotoDelivery { 
                        BookingId = bookings[2].BookingId, 
                        DeliveryMethod = "PhotographerDevice", 
                        DriveLink = "https://drive.google.com/drive/folders/2XYZ789ABC123DEF456GHI", 
                        DriveFolderName = "Carol_White_Wedding_Photos", 
                        PhotoCount = 120, 
                        Status = "Uploading", 
                        UploadedAt = DateTime.Now.AddHours(-2), 
                        DeliveredAt = null, 
                        ExpiresAt = DateTime.Now.AddDays(60), 
                        Notes = "Wedding photography session - uploading 120 photos", 
                        CreatedAt = DateTime.Now.AddDays(-2), 
                        UpdatedAt = DateTime.Now.AddHours(-2) 
                    },
                    
                    // PhotoDelivery 4 - Not required (customer took photos themselves)
                    new PhotoDelivery { 
                        BookingId = bookings[3].BookingId, 
                        DeliveryMethod = "CustomerDevice", 
                        DriveLink = null, 
                        DriveFolderName = null, 
                        PhotoCount = 0, 
                        Status = "NotRequired", 
                        UploadedAt = null, 
                        DeliveredAt = null, 
                        ExpiresAt = null, 
                        Notes = "Customer used their own device for photos", 
                        CreatedAt = DateTime.Now.AddDays(-1), 
                        UpdatedAt = DateTime.Now.AddDays(-1) 
                    }
                };
                context.PhotoDeliveries.AddRange(photoDeliveries);
                context.SaveChanges();
                Console.WriteLine($"PhotoDeliveries seeded successfully: {photoDeliveries.Length} photo deliveries added");
            }
            else
            {
                Console.WriteLine("PhotoDeliveries already exist, skipping...");
            }

            Console.WriteLine("=== Database Initialization Completed Successfully ===");
        }
    }
}
