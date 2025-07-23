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
                    nearbyPhotographers.Add(response);
                }
            }

            return nearbyPhotographers.OrderBy(p => CalculateDistance(latitude, longitude, p.Latitude ?? 0, p.Longitude ?? 0));
        }

        public async Task<IEnumerable<PhotographerListResponse>> GetPhotographersByCityAsync(string city)
        {
            var photographers = await _context.Photographers
                .Include(p => p.User)
                .Include(p => p.PhotographerStyles)
                .ThenInclude(ps => ps.Style)
                .Where(p => p.Address != null && p.Address.Contains(city, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();

            return _mapper.Map<IEnumerable<PhotographerListResponse>>(photographers);
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
    }
} 