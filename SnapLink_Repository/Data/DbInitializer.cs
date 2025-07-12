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
            context.Database.EnsureCreated();

            // Seed Roles
            if (!context.Roles.Any())
            {
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
            }

            // Seed Users
            if (!context.Users.Any())
            {
                var users = new[]
                {
                    new User { UserName = "alice", Email = "alice@example.com", PasswordHash = "hash1", PhoneNumber = "1234567890", FullName = "Alice Smith", Status = "Active", CreateAt = DateTime.Now },
                    new User { UserName = "bob", Email = "bob@example.com", PasswordHash = "hash2", PhoneNumber = "1234567891", FullName = "Bob Johnson", Status = "Active", CreateAt = DateTime.Now },
                    new User { UserName = "carol", Email = "carol@example.com", PasswordHash = "hash3", PhoneNumber = "1234567892", FullName = "Carol White", Status = "Active", CreateAt = DateTime.Now },
                    new User { UserName = "dave", Email = "dave@example.com", PasswordHash = "hash4", PhoneNumber = "1234567893", FullName = "Dave Brown", Status = "Active", CreateAt = DateTime.Now },
                    new User { UserName = "eve", Email = "eve@example.com", PasswordHash = "hash5", PhoneNumber = "1234567894", FullName = "Eve Black", Status = "Active", CreateAt = DateTime.Now },
                    new User { UserName = "frank", Email = "frank@example.com", PasswordHash = "hash6", PhoneNumber = "1234567895", FullName = "Frank Miller", Status = "Active", CreateAt = DateTime.Now },
                    new User { UserName = "grace", Email = "grace@example.com", PasswordHash = "hash7", PhoneNumber = "1234567896", FullName = "Grace Lee", Status = "Active", CreateAt = DateTime.Now },
                    new User { UserName = "henry", Email = "henry@example.com", PasswordHash = "hash8", PhoneNumber = "1234567897", FullName = "Henry Wilson", Status = "Active", CreateAt = DateTime.Now },
                    new User { UserName = "iris", Email = "iris@example.com", PasswordHash = "hash9", PhoneNumber = "1234567898", FullName = "Iris Davis", Status = "Active", CreateAt = DateTime.Now },
                    new User { UserName = "jack", Email = "jack@example.com", PasswordHash = "hash10", PhoneNumber = "1234567899", FullName = "Jack Taylor", Status = "Active", CreateAt = DateTime.Now },
                    new User { UserName = "kate", Email = "kate@example.com", PasswordHash = "hash11", PhoneNumber = "1234567900", FullName = "Kate Anderson", Status = "Active", CreateAt = DateTime.Now },
                    new User { UserName = "leo", Email = "leo@example.com", PasswordHash = "hash12", PhoneNumber = "1234567901", FullName = "Leo Martinez", Status = "Active", CreateAt = DateTime.Now },
                    new User { UserName = "maya", Email = "maya@example.com", PasswordHash = "hash13", PhoneNumber = "1234567902", FullName = "Maya Rodriguez", Status = "Active", CreateAt = DateTime.Now },
                    new User { UserName = "nina", Email = "nina@example.com", PasswordHash = "hash14", PhoneNumber = "1234567903", FullName = "Nina Thompson", Status = "Active", CreateAt = DateTime.Now },
                    new User { UserName = "oscar", Email = "oscar@example.com", PasswordHash = "hash15", PhoneNumber = "1234567904", FullName = "Oscar Garcia", Status = "Active", CreateAt = DateTime.Now },
                    // Additional users for location owners
                    new User { UserName = "peter", Email = "peter@example.com", PasswordHash = "hash16", PhoneNumber = "1234567905", FullName = "Peter Chen", Status = "Active", CreateAt = DateTime.Now },
                    new User { UserName = "sarah", Email = "sarah@example.com", PasswordHash = "hash17", PhoneNumber = "1234567906", FullName = "Sarah Kim", Status = "Active", CreateAt = DateTime.Now },
                    new User { UserName = "mike", Email = "mike@example.com", PasswordHash = "hash18", PhoneNumber = "1234567907", FullName = "Mike Johnson", Status = "Active", CreateAt = DateTime.Now },
                    new User { UserName = "lisa", Email = "lisa@example.com", PasswordHash = "hash19", PhoneNumber = "1234567908", FullName = "Lisa Wang", Status = "Active", CreateAt = DateTime.Now },
                    new User { UserName = "david", Email = "david@example.com", PasswordHash = "hash20", PhoneNumber = "1234567909", FullName = "David Park", Status = "Active", CreateAt = DateTime.Now }
                };
                context.Users.AddRange(users);
                context.SaveChanges();
            }

            // Seed UserRoles
            if (!context.UserRoles.Any())
            {
                var userRoles = new[]
                {
                    new UserRole { UserId = context.Users.First().UserId, RoleId = context.Roles.First(r => r.RoleName == "Admin").RoleId },
                    new UserRole { UserId = context.Users.Skip(1).First().UserId, RoleId = context.Roles.First(r => r.RoleName == "Photographer").RoleId },
                    new UserRole { UserId = context.Users.Skip(2).First().UserId, RoleId = context.Roles.First(r => r.RoleName == "User").RoleId },
                    new UserRole { UserId = context.Users.Skip(3).First().UserId, RoleId = context.Roles.First(r => r.RoleName == "Moderator").RoleId },
                    new UserRole { UserId = context.Users.Skip(4).First().UserId, RoleId = context.Roles.First(r => r.RoleName == "Owner").RoleId },
                    // Additional user roles for new location owners
                    new UserRole { UserId = context.Users.Skip(15).First().UserId, RoleId = context.Roles.First(r => r.RoleName == "Owner").RoleId }, // Peter
                    new UserRole { UserId = context.Users.Skip(16).First().UserId, RoleId = context.Roles.First(r => r.RoleName == "Owner").RoleId }, // Sarah
                    new UserRole { UserId = context.Users.Skip(17).First().UserId, RoleId = context.Roles.First(r => r.RoleName == "Owner").RoleId }, // Mike
                    new UserRole { UserId = context.Users.Skip(18).First().UserId, RoleId = context.Roles.First(r => r.RoleName == "Owner").RoleId }, // Lisa
                    new UserRole { UserId = context.Users.Skip(19).First().UserId, RoleId = context.Roles.First(r => r.RoleName == "Owner").RoleId }  // David
                };
                context.UserRoles.AddRange(userRoles);
                context.SaveChanges();
            }

            // Seed Styles
            if (!context.Styles.Any())
            {
                var styles = new[]
                {
                    new Style { Name = "Portrait", Description = "Portrait photography" },
                    new Style { Name = "Landscape", Description = "Landscape photography" },
                    new Style { Name = "Wedding", Description = "Wedding photography" },
                    new Style { Name = "Event", Description = "Event photography" },
                    new Style { Name = "Fashion", Description = "Fashion photography" }
                };
                context.Styles.AddRange(styles);
                context.SaveChanges();
            }

            // Seed Photographers
            if (!context.Photographers.Any())
            {
                var users = context.Users.Take(15).ToList();
                var photographers = new[]
                {
                    new Photographer { UserId = users[0].UserId, YearsExperience = 5, Equipment = "Canon EOS R5", HourlyRate = 100, AvailabilityStatus = "Available", Rating = 4.8M },
                    new Photographer { UserId = users[1].UserId, YearsExperience = 3, Equipment = "Nikon D850", HourlyRate = 80, AvailabilityStatus = "Available", Rating = 4.5M },
                    new Photographer { UserId = users[2].UserId, YearsExperience = 7, Equipment = "Sony A7 III", HourlyRate = 120, AvailabilityStatus = "Busy", Rating = 4.9M },
                    new Photographer { UserId = users[3].UserId, YearsExperience = 2, Equipment = "Fujifilm X-T4", HourlyRate = 70, AvailabilityStatus = "Available", Rating = 4.3M },
                    new Photographer { UserId = users[4].UserId, YearsExperience = 4, Equipment = "Olympus OM-D E-M1", HourlyRate = 90, AvailabilityStatus = "Available", Rating = 4.7M },
                    new Photographer { UserId = users[5].UserId, YearsExperience = 6, Equipment = "Canon EOS R6", HourlyRate = 110, AvailabilityStatus = "Available", Rating = 4.6M },
                    new Photographer { UserId = users[6].UserId, YearsExperience = 4, Equipment = "Nikon Z6", HourlyRate = 85, AvailabilityStatus = "Available", Rating = 4.4M },
                    new Photographer { UserId = users[7].UserId, YearsExperience = 8, Equipment = "Sony A7R IV", HourlyRate = 130, AvailabilityStatus = "Busy", Rating = 4.9M },
                    new Photographer { UserId = users[8].UserId, YearsExperience = 3, Equipment = "Fujifilm X-T3", HourlyRate = 75, AvailabilityStatus = "Available", Rating = 4.2M },
                    new Photographer { UserId = users[9].UserId, YearsExperience = 5, Equipment = "Canon EOS 5D", HourlyRate = 95, AvailabilityStatus = "Available", Rating = 4.5M },
                    new Photographer { UserId = users[10].UserId, YearsExperience = 7, Equipment = "Nikon D750", HourlyRate = 105, AvailabilityStatus = "Available", Rating = 4.7M },
                    new Photographer { UserId = users[11].UserId, YearsExperience = 2, Equipment = "Sony A6400", HourlyRate = 65, AvailabilityStatus = "Available", Rating = 4.1M },
                    new Photographer { UserId = users[12].UserId, YearsExperience = 6, Equipment = "Canon EOS 90D", HourlyRate = 115, AvailabilityStatus = "Busy", Rating = 4.8M },
                    new Photographer { UserId = users[13].UserId, YearsExperience = 4, Equipment = "Nikon Z50", HourlyRate = 80, AvailabilityStatus = "Available", Rating = 4.3M },
                    new Photographer { UserId = users[14].UserId, YearsExperience = 9, Equipment = "Sony A9", HourlyRate = 140, AvailabilityStatus = "Available", Rating = 4.9M }
                };
                context.Photographers.AddRange(photographers);
                context.SaveChanges();
            }

            // Seed PhotographerStyles
            if (!context.PhotographerStyles.Any())
            {
                var photographers = context.Photographers.Take(15).ToList();
                var styles = context.Styles.Take(5).ToList();
                var photographerStyles = new[]
                {
                    // Photographer 1 (Alice) - Portrait specialist
                    new PhotographerStyle { PhotographerId = photographers[0].PhotographerId, StyleId = styles[0].StyleId },
                    
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
            }

            // Seed LocationOwners
            if (!context.LocationOwners.Any())
            {
                var users = context.Users.Take(10).ToList(); // Take 10 users (5 original + 5 new)
                var owners = new[]
                {
                    new LocationOwner { UserId = users[0].UserId, BusinessName = "VenueCo", BusinessAddress = "venueco@example.com" },
                    new LocationOwner { UserId = users[1].UserId, BusinessName = "EventSpaces", BusinessAddress = "eventspaces@example.com" },
                    new LocationOwner { UserId = users[2].UserId, BusinessName = "PhotoStudios", BusinessAddress = "photostudios@example.com" },
                    new LocationOwner { UserId = users[3].UserId, BusinessName = "UrbanVenues", BusinessAddress = "urbanvenues@example.com" },
                    new LocationOwner { UserId = users[4].UserId, BusinessName = "NatureSpots", BusinessAddress = "naturespots@example.com" },
                    // Additional location owners
                    new LocationOwner { UserId = users[5].UserId, BusinessName = "Studio Elite", BusinessAddress = "studioelite@example.com" },
                    new LocationOwner { UserId = users[6].UserId, BusinessName = "Creative Spaces", BusinessAddress = "creativespaces@example.com" },
                    new LocationOwner { UserId = users[7].UserId, BusinessName = "Modern Venues", BusinessAddress = "modernvenues@example.com" },
                    new LocationOwner { UserId = users[8].UserId, BusinessName = "Premium Studios", BusinessAddress = "premiumstudios@example.com" },
                    new LocationOwner { UserId = users[9].UserId, BusinessName = "Luxury Locations", BusinessAddress = "luxurylocations@example.com" }
                };
                context.LocationOwners.AddRange(owners);
                context.SaveChanges();
            }

            // Seed Locations
            if (!context.Locations.Any())
            {
                var owners = context.LocationOwners.Take(10).ToList(); // Take all 10 location owners
                var locations = new[]
                {
                    // Original 5 registered locations with fees
                    new Location { 
                        LocationOwnerId = owners[0].LocationOwnerId, 
                        Name = "Central Park Studio", 
                        Address = "123 Main St, New York, NY", 
                        Description = "Spacious studio downtown", 
                        HourlyRate = 50, 
                        Capacity = 20, 
                        Indoor = true, 
                        Outdoor = false, 
                        AvailabilityStatus = "Available", 
                        LocationType = "Registered",
                        ExternalPlaceId = "ChIJKxjxuxlZwokRwA2Ire1V8mk", // Example Google Places ID
                        CreatedAt = DateTime.Now 
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
                        CreatedAt = DateTime.Now 
                    },
                    new Location { 
                        LocationOwnerId = owners[2].LocationOwnerId, 
                        Name = "Urban Loft", 
                        Address = "789 City Ave, New York, NY", 
                        Description = "Modern loft in city center", 
                        HourlyRate = 60, 
                        Capacity = 30, 
                        Indoor = true, 
                        Outdoor = false, 
                        AvailabilityStatus = "Busy", 
                        LocationType = "Registered",
                        ExternalPlaceId = "ChIJN1t_tDeuEmsRUsoyG83frY4", // Example Google Places ID
                        CreatedAt = DateTime.Now 
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
                        CreatedAt = DateTime.Now 
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
                        CreatedAt = DateTime.Now 
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
                        CreatedAt = DateTime.Now 
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
                        CreatedAt = DateTime.Now 
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
                        CreatedAt = DateTime.Now 
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
                        CreatedAt = DateTime.Now 
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
                        CreatedAt = DateTime.Now 
                    },
                    // External locations (Google Places) - no fees
                    new Location { 
                        LocationOwnerId = owners[0].LocationOwnerId, 
                        Name = "Central Park", 
                        Address = "Central Park, New York, NY", 
                        Description = "Iconic urban park with beautiful landscapes", 
                        HourlyRate = 0, // No fee for external locations
                        Capacity = 100, 
                        Indoor = false, 
                        Outdoor = true, 
                        AvailabilityStatus = "Available", 
                        LocationType = "External",
                        ExternalPlaceId = "ChIJN1t_tDeuEmsRUsoyG83frY4", // Google Places ID for Central Park
                        CreatedAt = DateTime.Now 
                    },
                    new Location { 
                        LocationOwnerId = owners[1].LocationOwnerId, 
                        Name = "Times Square", 
                        Address = "Times Square, New York, NY", 
                        Description = "Famous commercial intersection and tourist destination", 
                        HourlyRate = 0, // No fee for external locations
                        Capacity = 200, 
                        Indoor = false, 
                        Outdoor = true, 
                        AvailabilityStatus = "Available", 
                        LocationType = "External",
                        ExternalPlaceId = "ChIJaXQRs6lZwokRY6EFpJnhNNE", // Google Places ID for Times Square
                        CreatedAt = DateTime.Now 
                    },
                    new Location { 
                        LocationOwnerId = owners[2].LocationOwnerId, 
                        Name = "Brooklyn Bridge", 
                        Address = "Brooklyn Bridge, New York, NY", 
                        Description = "Historic suspension bridge connecting Manhattan and Brooklyn", 
                        HourlyRate = 0, // No fee for external locations
                        Capacity = 50, 
                        Indoor = false, 
                        Outdoor = true, 
                        AvailabilityStatus = "Available", 
                        LocationType = "External",
                        ExternalPlaceId = "ChIJKxjxuxlZwokRwA2Ire1V8mk", // Google Places ID for Brooklyn Bridge
                        CreatedAt = DateTime.Now 
                    }
                };
                context.Locations.AddRange(locations);
                context.SaveChanges();
            }

            // Seed Bookings
            if (!context.Bookings.Any())
            {
                var users = context.Users.Take(5).ToList();
                var photographers = context.Photographers.Take(5).ToList();
                var locations = context.Locations.Take(8).ToList(); // Now we have 8 locations (5 registered + 3 external)
                var bookings = new[]
                {
                    // Bookings with registered locations (include location fees)
                    new Booking { 
                        UserId = users[0].UserId, 
                        PhotographerId = photographers[0].PhotographerId, 
                        LocationId = locations[0].LocationId, // Central Park Studio (Registered)
                        StartDatetime = DateTime.Now.AddDays(1), 
                        EndDatetime = DateTime.Now.AddDays(1).AddHours(2), 
                        Status = "Confirmed", 
                        TotalPrice = 300, // Photographer (100/hr * 2) + Location (50/hr * 2) = 300
                        CreatedAt = DateTime.Now 
                    },
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
            }

            // Seed Reviews
            if (!context.Reviews.Any())
            {
                var bookings = context.Bookings.Take(5).ToList();
                var reviews = new[]
                {
                    new Review { BookingId = bookings[0].BookingId, Rating = 5, Comment = "Excellent!", CreatedAt = DateTime.Now },
                    new Review { BookingId = bookings[1].BookingId, Rating = 4, Comment = "Very good", CreatedAt = DateTime.Now },
                    new Review { BookingId = bookings[2].BookingId, Rating = 5, Comment = "Outstanding service", CreatedAt = DateTime.Now },
                    new Review { BookingId = bookings[3].BookingId, Rating = 3, Comment = "Average", CreatedAt = DateTime.Now },
                    new Review { BookingId = bookings[4].BookingId, Rating = 4, Comment = "Good experience", CreatedAt = DateTime.Now }
                };
                context.Reviews.AddRange(reviews);
                context.SaveChanges();
            }

            // Seed Payments
            if (!context.Payments.Any())
            {
                var bookings = context.Bookings.Take(5).ToList();
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
                        ExternalTransactionId = "PAY_001",
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
                        ExternalTransactionId = "PAY_003",
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
                        ExternalTransactionId = "PAY_004",
                        Note = "Payment for external location booking",
                        CreatedAt = DateTime.Now 
                    },
                    // Payment for external location booking (Times Square - no location fee)
                    new Payment { 
                        CustomerId = users[4].UserId,
                        BookingId = bookings[4].BookingId, 
                        TotalAmount = 180, 
                        Status = PaymentStatus.Success, 
                        Method = "PayOS",
                        ExternalTransactionId = "PAY_005",
                        Note = "Payment for external location booking",
                        CreatedAt = DateTime.Now 
                    }
                };
                context.Payments.AddRange(payments);
                context.SaveChanges();
            }

            // Seed Notifications
            if (!context.Notifications.Any())
            {
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
            }

            // Seed Administrators
            if (!context.Administrators.Any())
            {
                var users = context.Users.Take(5).ToList();
                var admins = new[]
                {
                    new Administrator { UserId = users[0].UserId, AccessLevel = "Super", Department = "IT" },
                    new Administrator { UserId = users[1].UserId, AccessLevel = "Standard", Department = "Support" },
                    new Administrator { UserId = users[2].UserId, AccessLevel = "Super", Department = "HR" },
                    new Administrator { UserId = users[3].UserId, AccessLevel = "Standard", Department = "Finance" },
                    new Administrator { UserId = users[4].UserId, AccessLevel = "Standard", Department = "Marketing" }
                };
                context.Administrators.AddRange(admins);
                context.SaveChanges();
            }

            // Seed Moderators
            if (!context.Moderators.Any())
            {
                var users = context.Users.Take(5).ToList();
                var moderators = new[]
                {
                    new Moderator { UserId = users[0].UserId, AreasOfFocus = "Abuse" },
                    new Moderator { UserId = users[1].UserId, AreasOfFocus = "Spam" },
                    new Moderator { UserId = users[2].UserId, AreasOfFocus = "Payments" },
                    new Moderator { UserId = users[3].UserId, AreasOfFocus = "Bookings" },
                    new Moderator { UserId = users[4].UserId, AreasOfFocus = "General" }
                };
                context.Moderators.AddRange(moderators);
                context.SaveChanges();
            }

            // Seed Messagess
            if (!context.Messagesses.Any())
            {
                var users = context.Users.Take(5).ToList();
                var messages = new[]
                {
                    new Messagess { SenderId = users[0].UserId, RecipientId = users[1].UserId, Content = "Hi Bob!", CreatedAt = DateTime.Now },
                    new Messagess { SenderId = users[1].UserId, RecipientId = users[2].UserId, Content = "Hello Carol!", CreatedAt = DateTime.Now },
                    new Messagess { SenderId = users[2].UserId, RecipientId = users[3].UserId, Content = "Hey Dave!", CreatedAt = DateTime.Now },
                    new Messagess { SenderId = users[3].UserId, RecipientId = users[4].UserId, Content = "Hi Eve!", CreatedAt = DateTime.Now },
                    new Messagess { SenderId = users[4].UserId, RecipientId = users[0].UserId, Content = "Hello Alice!", CreatedAt = DateTime.Now }
                };
                context.Messagesses.AddRange(messages);
                context.SaveChanges();
            }

            // Seed PremiumPackages
            if (!context.PremiumPackages.Any())
            {
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
            }

            // Seed PremiumSubscriptions
            if (!context.PremiumSubscriptions.Any())
            {
                var users = context.Users.Take(5).ToList();
                var packages = context.PremiumPackages.Take(5).ToList();
                var subscriptions = new[]
                {
                    new PremiumSubscription { UserId = users[0].UserId, PackageId = packages[0].PackageId, StartDate = DateTime.Now.AddDays(-10), EndDate = DateTime.Now.AddDays(20), Status = "Active" },
                    new PremiumSubscription { UserId = users[1].UserId, PackageId = packages[1].PackageId, StartDate = DateTime.Now.AddDays(-20), EndDate = DateTime.Now.AddDays(10), Status = "Expired" },
                    new PremiumSubscription { UserId = users[2].UserId, PackageId = packages[2].PackageId, StartDate = DateTime.Now.AddDays(-5), EndDate = DateTime.Now.AddDays(25), Status = "Active" },
                    new PremiumSubscription { UserId = users[3].UserId, PackageId = packages[3].PackageId, StartDate = DateTime.Now.AddDays(-15), EndDate = DateTime.Now.AddDays(15), Status = "Active" },
                    new PremiumSubscription { UserId = users[4].UserId, PackageId = packages[4].PackageId, StartDate = DateTime.Now.AddDays(-30), EndDate = DateTime.Now.AddDays(5), Status = "Expired" }
                };
                context.PremiumSubscriptions.AddRange(subscriptions);
                context.SaveChanges();
            }

            // Seed Transactions
            if (!context.Transactions.Any())
            {
                var users = context.Users.Take(5).ToList();
                var transactions = new[]
                {
                    new Transaction { FromUserId = null, ToUserId = users[0].UserId, Amount = 100, Type = TransactionType.Topup, Status = TransactionStatus.Success, Note = "Initial deposit", CreatedAt = DateTime.Now },
                    new Transaction { FromUserId = users[1].UserId, ToUserId = null, Amount = 50, Type = TransactionType.Withdraw, Status = TransactionStatus.Success, Note = "Withdrawal", CreatedAt = DateTime.Now },
                    new Transaction { FromUserId = null, ToUserId = users[2].UserId, Amount = 75, Type = TransactionType.Refund, Status = TransactionStatus.Success, Note = "Refund for cancelled booking", CreatedAt = DateTime.Now },
                    new Transaction { FromUserId = users[3].UserId, ToUserId = null, Amount = 120, Type = TransactionType.Payout, Status = TransactionStatus.Pending, Note = "Photographer payout", CreatedAt = DateTime.Now },
                    new Transaction { FromUserId = null, ToUserId = users[4].UserId, Amount = 200, Type = TransactionType.Bonus, Status = TransactionStatus.Success, Note = "Referral bonus", CreatedAt = DateTime.Now }
                };
                context.Transactions.AddRange(transactions);
                context.SaveChanges();
            }

            // Seed Wallets
            if (!context.Wallets.Any())
            {
                var users = context.Users.Take(20).ToList(); // Take all 20 users
                var wallets = new[]
                {
                    new Wallet { UserId = users[0].UserId, Balance = 500, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[1].UserId, Balance = 300, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[2].UserId, Balance = 700, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[3].UserId, Balance = 200, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[4].UserId, Balance = 400, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[5].UserId, Balance = 150, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[6].UserId, Balance = 600, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[7].UserId, Balance = 250, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[8].UserId, Balance = 350, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[9].UserId, Balance = 450, UpdatedAt = DateTime.Now },
                    // Additional wallets for new users
                    new Wallet { UserId = users[10].UserId, Balance = 800, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[11].UserId, Balance = 550, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[12].UserId, Balance = 650, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[13].UserId, Balance = 750, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[14].UserId, Balance = 900, UpdatedAt = DateTime.Now },
                    new Wallet { UserId = users[15].UserId, Balance = 1200, UpdatedAt = DateTime.Now }, // Peter - Location Owner
                    new Wallet { UserId = users[16].UserId, Balance = 1100, UpdatedAt = DateTime.Now }, // Sarah - Location Owner
                    new Wallet { UserId = users[17].UserId, Balance = 1300, UpdatedAt = DateTime.Now }, // Mike - Location Owner
                    new Wallet { UserId = users[18].UserId, Balance = 1400, UpdatedAt = DateTime.Now }, // Lisa - Location Owner
                    new Wallet { UserId = users[19].UserId, Balance = 1600, UpdatedAt = DateTime.Now }  // David - Location Owner
                };
                context.Wallets.AddRange(wallets);
                context.SaveChanges();
            }

            // Seed WithdrawalRequests
            if (!context.WithdrawalRequests.Any())
            {
                var photographers = context.Photographers.Take(5).ToList();
                var requests = new[]
                {
                    new WithdrawalRequest { PhotographerId = photographers[0].PhotographerId, Amount = 100, BankAccountNumber = "111111", BankAccountName = "Alice Smith", BankName = "Bank A", RequestStatus = "Pending", RequestedAt = DateTime.Now },
                    new WithdrawalRequest { PhotographerId = photographers[1].PhotographerId, Amount = 200, BankAccountNumber = "222222", BankAccountName = "Bob Johnson", BankName = "Bank B", RequestStatus = "Approved", RequestedAt = DateTime.Now },
                    new WithdrawalRequest { PhotographerId = photographers[2].PhotographerId, Amount = 150, BankAccountNumber = "333333", BankAccountName = "Carol White", BankName = "Bank C", RequestStatus = "Rejected", RequestedAt = DateTime.Now },
                    new WithdrawalRequest { PhotographerId = photographers[3].PhotographerId, Amount = 120, BankAccountNumber = "444444", BankAccountName = "Dave Brown", BankName = "Bank D", RequestStatus = "Pending", RequestedAt = DateTime.Now },
                    new WithdrawalRequest { PhotographerId = photographers[4].PhotographerId, Amount = 180, BankAccountNumber = "555555", BankAccountName = "Eve Black", BankName = "Bank E", RequestStatus = "Approved", RequestedAt = DateTime.Now }
                };
                context.WithdrawalRequests.AddRange(requests);
                context.SaveChanges();
            }

            // Seed Complaints
            if (!context.Complaints.Any())
            {
                var users = context.Users.Take(5).ToList();
                var bookings = context.Bookings.Take(5).ToList();
                var moderators = context.Moderators.Take(5).ToList();
                var complaints = new[]
                {
                    new Complaint { ReporterId = users[0].UserId, ReportedUserId = users[1].UserId, BookingId = bookings[0].BookingId, ComplaintType = "Service", Description = "Late arrival", Status = "Open", AssignedModeratorId = moderators[0].ModeratorId, CreatedAt = DateTime.Now },
                    new Complaint { ReporterId = users[1].UserId, ReportedUserId = users[2].UserId, BookingId = bookings[1].BookingId, ComplaintType = "Payment", Description = "Overcharged", Status = "Closed", AssignedModeratorId = moderators[1].ModeratorId, CreatedAt = DateTime.Now },
                    new Complaint { ReporterId = users[2].UserId, ReportedUserId = users[3].UserId, BookingId = bookings[2].BookingId, ComplaintType = "Behavior", Description = "Rude behavior", Status = "Open", AssignedModeratorId = moderators[2].ModeratorId, CreatedAt = DateTime.Now },
                    new Complaint { ReporterId = users[3].UserId, ReportedUserId = users[4].UserId, BookingId = bookings[3].BookingId, ComplaintType = "Quality", Description = "Low quality photos", Status = "Closed", AssignedModeratorId = moderators[3].ModeratorId, CreatedAt = DateTime.Now },
                    new Complaint { ReporterId = users[4].UserId, ReportedUserId = users[0].UserId, BookingId = bookings[4].BookingId, ComplaintType = "Other", Description = "Other issue", Status = "Open", AssignedModeratorId = moderators[4].ModeratorId, CreatedAt = DateTime.Now }
                };
                context.Complaints.AddRange(complaints);
                context.SaveChanges();
            }

            // Seed Advertisements
            if (!context.Advertisements.Any())
            {
                var locations = context.Locations.Take(5).ToList();
                var payments = context.Payments.Take(5).ToList();
                var ads = new[]
                {
                    new Advertisement { LocationId = locations[0].LocationId, Title = "Grand Opening", Description = "Special offer for new customers", ImageUrl = "url1.jpg", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30), Status = "Active", Cost = 100, PaymentId = payments[0].PaymentId },
                    new Advertisement { LocationId = locations[1].LocationId, Title = "Summer Sale", Description = "Discounts on bookings", ImageUrl = "url2.jpg", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(15), Status = "Active", Cost = 80, PaymentId = payments[1].PaymentId },
                    new Advertisement { LocationId = locations[2].LocationId, Title = "Wedding Season", Description = "Book now for weddings", ImageUrl = "url3.jpg", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(45), Status = "Inactive", Cost = 120, PaymentId = payments[2].PaymentId },
                    new Advertisement { LocationId = locations[3].LocationId, Title = "Photo Contest", Description = "Join and win prizes", ImageUrl = "url4.jpg", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(10), Status = "Active", Cost = 60, PaymentId = payments[3].PaymentId },
                    new Advertisement { LocationId = locations[4].LocationId, Title = "Holiday Shoots", Description = "Special holiday packages", ImageUrl = "url5.jpg", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(20), Status = "Active", Cost = 90, PaymentId = payments[4].PaymentId }
                };
                context.Advertisements.AddRange(ads);
                context.SaveChanges();
            }

            // Seed PhotographerEvents
            if (!context.PhotographerEvents.Any())
            {
                var photographers = context.Photographers.Take(5).ToList();
                var events = new[]
                {
                    new PhotographerEvent { PhotographerId = photographers[0].PhotographerId, Title = "Spring Mini Sessions", Description = "Short sessions for families", OriginalPrice = 200, DiscountedPrice = 150, DiscountPercentage = 25, StartDate = DateTime.Now.AddDays(5), EndDate = DateTime.Now.AddDays(10), MaxBookings = 10, CurrentBookings = 2, Status = "Active", CreatedAt = DateTime.Now },
                    new PhotographerEvent { PhotographerId = photographers[1].PhotographerId, Title = "Wedding Expo", Description = "Meet top wedding photographers", OriginalPrice = 500, DiscountedPrice = 400, DiscountPercentage = 20, StartDate = DateTime.Now.AddDays(15), EndDate = DateTime.Now.AddDays(20), MaxBookings = 20, CurrentBookings = 5, Status = "Active", CreatedAt = DateTime.Now },
                    new PhotographerEvent { PhotographerId = photographers[2].PhotographerId, Title = "Landscape Workshop", Description = "Learn landscape photography", OriginalPrice = 300, DiscountedPrice = 250, DiscountPercentage = 17, StartDate = DateTime.Now.AddDays(25), EndDate = DateTime.Now.AddDays(30), MaxBookings = 15, CurrentBookings = 3, Status = "Inactive", CreatedAt = DateTime.Now },
                    new PhotographerEvent { PhotographerId = photographers[3].PhotographerId, Title = "Fashion Shoot", Description = "Fashion photography for models", OriginalPrice = 400, DiscountedPrice = 350, DiscountPercentage = 12, StartDate = DateTime.Now.AddDays(35), EndDate = DateTime.Now.AddDays(40), MaxBookings = 8, CurrentBookings = 1, Status = "Active", CreatedAt = DateTime.Now },
                    new PhotographerEvent { PhotographerId = photographers[4].PhotographerId, Title = "Event Coverage", Description = "Book for your event", OriginalPrice = 250, DiscountedPrice = 200, DiscountPercentage = 20, StartDate = DateTime.Now.AddDays(45), EndDate = DateTime.Now.AddDays(50), MaxBookings = 12, CurrentBookings = 4, Status = "Active", CreatedAt = DateTime.Now }
                };
                context.PhotographerEvents.AddRange(events);
                context.SaveChanges();
            }

            // Seed PhotographerEventLocations
            if (!context.PhotographerEventLocations.Any())
            {
                var events = context.PhotographerEvents.Take(5).ToList();
                var locations = context.Locations.Take(5).ToList();
                var eventLocations = new[]
                {
                    new PhotographerEventLocation { EventId = events[0].EventId, LocationId = locations[0].LocationId },
                    new PhotographerEventLocation { EventId = events[1].EventId, LocationId = locations[1].LocationId },
                    new PhotographerEventLocation { EventId = events[2].EventId, LocationId = locations[2].LocationId },
                    new PhotographerEventLocation { EventId = events[3].EventId, LocationId = locations[3].LocationId },
                    new PhotographerEventLocation { EventId = events[4].EventId, LocationId = locations[4].LocationId }
                };
                context.PhotographerEventLocations.AddRange(eventLocations);
                context.SaveChanges();
            }

            // Seed Images
            if (!context.Images.Any())
            {
                var photographers = context.Photographers.Take(5).ToList();
                var locations = context.Locations.Take(5).ToList();
                var images = new[]
                {
                    new Image { Url = "photographer1.jpg", Type = "photographer", RefId = photographers[0].PhotographerId, IsPrimary = true, Caption = "Photographer 1", CreatedAt = DateTime.Now },
                    new Image { Url = "photographer2.jpg", Type = "photographer", RefId = photographers[1].PhotographerId, IsPrimary = true, Caption = "Photographer 2", CreatedAt = DateTime.Now },
                    new Image { Url = "location1.jpg", Type = "location", RefId = locations[0].LocationId, IsPrimary = true, Caption = "Location 1", CreatedAt = DateTime.Now },
                    new Image { Url = "location2.jpg", Type = "location", RefId = locations[1].LocationId, IsPrimary = true, Caption = "Location 2", CreatedAt = DateTime.Now },
                    new Image { Url = "location3.jpg", Type = "location", RefId = locations[2].LocationId, IsPrimary = false, Caption = "Location 3", CreatedAt = DateTime.Now }
                };
                context.Images.AddRange(images);
                context.SaveChanges();
            }

            // Seed UserStyles
            if (!context.UserStyles.Any())
            {
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
            }
        }
    }
}
