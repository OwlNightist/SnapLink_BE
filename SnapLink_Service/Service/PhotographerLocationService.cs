using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SnapLink_Model.DTO.Response;
using SnapLink_Repository.DBContext;
using SnapLink_Repository.Entity;
using SnapLink_Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnapLink_Service.Service
{
    public class PhotographerLocationService : IPhotographerLocationService
    {
        private readonly SnaplinkDbContext _context;
        private readonly IMapper _mapper;

        public PhotographerLocationService(SnaplinkDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PhotographerListResponse>> GetPhotographersWithinRadiusAsync(double latitude, double longitude, double radiusKm)
        {
            var photographers = await _context.Photographers
                .Include(p => p.User)
                .Include(p => p.PhotographerStyles)
                .ThenInclude(ps => ps.Style)
                .Where(p => p.Latitude.HasValue && p.Longitude.HasValue)
                .ToListAsync();

            var nearbyPhotographers = new List<PhotographerListResponse>();

            foreach (var photographer in photographers)
            {
                var distance = CalculateDistance(latitude, longitude, photographer.Latitude.Value, photographer.Longitude.Value);
                if (distance <= radiusKm)
                {
                    var response = _mapper.Map<PhotographerListResponse>(photographer);
                    response.DistanceKm = distance;
                    nearbyPhotographers.Add(response);
                }
            }

            return nearbyPhotographers.OrderBy(p => p.DistanceKm);
        }


        public async Task<double> CalculateDistanceToPhotographerAsync(double userLat, double userLon, int photographerId)
        {
            var photographer = await _context.Photographers
                .FirstOrDefaultAsync(p => p.PhotographerId == photographerId);

            if (photographer?.Latitude == null || photographer?.Longitude == null)
                throw new InvalidOperationException("Photographer location not found");

            return CalculateDistance(userLat, userLon, photographer.Latitude.Value, photographer.Longitude.Value);
        }

        public async Task<bool> ValidatePhotographerLocationAsync(int photographerId, double latitude, double longitude)
        {
            var photographer = await _context.Photographers
                .FirstOrDefaultAsync(p => p.PhotographerId == photographerId);

            if (photographer?.Latitude == null || photographer?.Longitude == null)
                return false;

            // Check if the provided coordinates are within a reasonable distance (e.g., 1km) of the stored location
            var distance = CalculateDistance(latitude, longitude, photographer.Latitude.Value, photographer.Longitude.Value);
            return distance <= 1.0; // 1km tolerance
        }

        public async Task UpdatePhotographerLocationAsync(int photographerId, string address, string googleMapsAddress, double? latitude, double? longitude)
        {
            var photographer = await _context.Photographers
                .FirstOrDefaultAsync(p => p.PhotographerId == photographerId);

            if (photographer == null)
                throw new InvalidOperationException("Photographer not found");

            photographer.Address = address;
            photographer.GoogleMapsAddress = googleMapsAddress;
            photographer.Latitude = latitude;
            photographer.Longitude = longitude;

            await _context.SaveChangesAsync();
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Earth's radius in kilometers
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public async Task<IEnumerable<PhotographerListResponse>> GetRecommendedPhotographersAsync(double latitude, double longitude, int userId, int? locationId = null, double radiusKm = 10.0, int maxResults = 20)
        {
            // Get user's preferred styles
            var userStyles = await _context.UserStyles
                .Where(us => us.UserId == userId)
                .Select(us => us.StyleId)
                .ToListAsync();

            // Get all photographers within radius with their details
            var photographers = await _context.Photographers
                .Include(p => p.User)
                .Include(p => p.PhotographerStyles)
                .ThenInclude(ps => ps.Style)
                .Include(p => p.Bookings)
                .Where(p => p.Latitude.HasValue && p.Longitude.HasValue)
                .ToListAsync();

            var recommendedPhotographers = new List<PhotographerListResponse>();

            foreach (var photographer in photographers)
            {
                var distance = CalculateDistance(latitude, longitude, photographer.Latitude.Value, photographer.Longitude.Value);
                if (distance <= radiusKm)
                {
                    var response = _mapper.Map<PhotographerListResponse>(photographer);
                    response.DistanceKm = distance;

                    // Calculate style match score (number of matching styles)
                    var photographerStyleIds = photographer.PhotographerStyles.Select(ps => ps.StyleId).ToList();
                    var styleMatchCount = userStyles.Intersect(photographerStyleIds).Count();
                    
                    // Check if photographer has been booked at the specific location
                    bool isBookedAtLocation = false;
                    if (locationId.HasValue)
                    {
                        isBookedAtLocation = photographer.Bookings.Any(b => b.LocationId == locationId.Value);
                    }
                    response.Isbookedhere = isBookedAtLocation;

                    // Create a temporary object to hold sorting criteria
                    var photographerWithScore = new
                    {
                        Photographer = response,
                        StyleMatchCount = styleMatchCount,
                        IsBookedAtLocation = isBookedAtLocation,
                        Rating = photographer.Rating ?? 0,
                        Distance = distance
                    };

                    recommendedPhotographers.Add(response);
                }
            }

            // Sort by priority (consistent with by-user-styles API):
            // 1. Style match count (3 matches > 2 matches > 1 match)
            // 2. Higher rating within same match count
            // 3. Booked at location (if locationId provided)
            // 4. Closer distance for same rating
            var sortedPhotographers = recommendedPhotographers
                .Select(p => new
                {
                    Photographer = p,
                    StyleMatchCount = userStyles.Intersect(
                        photographers.First(ph => ph.PhotographerId == p.PhotographerId)
                            .PhotographerStyles.Select(ps => ps.StyleId)
                    ).Count(),
                    IsBookedAtLocation = p.Isbookedhere ?? false,
                    Rating = p.Rating ?? 0,
                    Distance = p.DistanceKm ?? 0
                })
                .OrderByDescending(x => x.StyleMatchCount)  // 3 matches > 2 matches > 1 match
                .ThenByDescending(x => x.Rating)            // Higher rating within same match count
                .ThenByDescending(x => x.IsBookedAtLocation) // Booked at location advantage
                .ThenBy(x => x.Distance)                    // Closer distance for same rating
                .Take(maxResults)
                .Select(x => x.Photographer);

            return sortedPhotographers;
        }

        public async Task<IEnumerable<PhotographerListResponse>> GetPhotographersByUserStylesAsync(int userId, double userLatitude, double userLongitude)
        {
            // Get user's preferred styles
            var userStyles = await _context.UserStyles
                .Where(us => us.UserId == userId)
                .Select(us => us.StyleId)
                .ToListAsync();

            if (!userStyles.Any())
            {
                // If user has no preferred styles, return empty list
                return new List<PhotographerListResponse>();
            }

            // Get photographers who have at least one matching style and have location data
            var photographers = await _context.Photographers
                .Include(p => p.User)
                .Include(p => p.PhotographerStyles)
                .ThenInclude(ps => ps.Style)
                .Where(p => p.PhotographerStyles.Any(ps => userStyles.Contains(ps.StyleId)) 
                           && p.Latitude.HasValue && p.Longitude.HasValue)
                .ToListAsync();

            var photographerResponses = photographers
                .Select(p => {
                    var response = _mapper.Map<PhotographerListResponse>(p);
                    
                    // Calculate style match count
                    var photographerStyleIds = p.PhotographerStyles.Select(ps => ps.StyleId).ToList();
                    var styleMatchCount = userStyles.Intersect(photographerStyleIds).Count();
                    
                    // Calculate distance using existing method
                    var distance = CalculateDistance(userLatitude, userLongitude, p.Latitude.Value, p.Longitude.Value);
                    response.DistanceKm = distance;
                    
                    return new { 
                        Photographer = response, 
                        StyleMatchCount = styleMatchCount,
                        Rating = p.Rating ?? 0,
                        Distance = distance
                    };
                })
                .OrderByDescending(x => x.StyleMatchCount)  // 3 matches > 2 matches > 1 match
                .ThenByDescending(x => x.Rating)            // Higher rating within same match count
                .ThenBy(x => x.Distance)                    // Closer distance for same rating
                .Select(x => x.Photographer);

            return photographerResponses;
        }
    }
} 