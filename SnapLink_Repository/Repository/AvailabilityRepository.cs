using Microsoft.EntityFrameworkCore;
using SnapLink_Repository.DBContext;
using SnapLink_Repository.Entity;
using SnapLink_Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnapLink_Repository.Repository
{
    public class AvailabilityRepository : IAvailabilityRepository
    {
        private readonly SnaplinkDbContext _context;

        public AvailabilityRepository(SnaplinkDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Availability>> GetAvailabilitiesByPhotographerIdAsync(int photographerId)
        {
            return await _context.Availabilities
                .Where(a => a.PhotographerId == photographerId)
                .OrderBy(a => a.DayOfWeek)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Availability>> GetAvailabilitiesByDayOfWeekAsync(DayOfWeek dayOfWeek)
        {
            return await _context.Availabilities
                .Where(a => a.DayOfWeek == dayOfWeek && a.Status == "Available")
                .Include(a => a.Photographer)
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Availability>> GetAvailableSlotsAsync(int photographerId, DayOfWeek dayOfWeek)
        {
            return await _context.Availabilities
                .Where(a => a.PhotographerId == photographerId && 
                           a.DayOfWeek == dayOfWeek && 
                           a.Status == "Available")
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<bool> IsTimeSlotAvailableAsync(int photographerId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            var conflictingAvailabilities = await _context.Availabilities
                .Where(a => a.PhotographerId == photographerId &&
                           a.DayOfWeek == dayOfWeek &&
                           a.Status == "Available" &&
                           ((a.StartTime <= startTime && a.EndTime > startTime) ||
                            (a.StartTime < endTime && a.EndTime >= endTime) ||
                            (a.StartTime >= startTime && a.EndTime <= endTime)))
                .AnyAsync();

            return !conflictingAvailabilities;
        }

        public async Task<IEnumerable<Availability>> GetConflictingAvailabilitiesAsync(int photographerId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            return await _context.Availabilities
                .Where(a => a.PhotographerId == photographerId &&
                           a.DayOfWeek == dayOfWeek &&
                           a.Status == "Available" &&
                           ((a.StartTime <= startTime && a.EndTime > startTime) ||
                            (a.StartTime < endTime && a.EndTime >= endTime) ||
                            (a.StartTime >= startTime && a.EndTime <= endTime)))
                .ToListAsync();
        }

        public async Task<IEnumerable<Availability>> GetAvailabilitiesByStatusAsync(string status)
        {
            return await _context.Availabilities
                .Where(a => a.Status == status)
                .Include(a => a.Photographer)
                .OrderBy(a => a.PhotographerId)
                .ThenBy(a => a.DayOfWeek)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<Availability?> GetAvailabilityByIdAsync(int availabilityId)
        {
            return await _context.Availabilities
                .Include(a => a.Photographer)
                .FirstOrDefaultAsync(a => a.AvailabilityId == availabilityId);
        }

        public async Task AddAvailabilityAsync(Availability availability)
        {
            availability.CreatedAt = DateTime.UtcNow;
            await _context.Availabilities.AddAsync(availability);
        }

        public async Task UpdateAvailabilityAsync(Availability availability)
        {
            availability.UpdatedAt = DateTime.UtcNow;
            _context.Availabilities.Update(availability);
        }

        public async Task DeleteAvailabilityAsync(Availability availability)
        {
            _context.Availabilities.Remove(availability);
        }

        public async Task DeleteAvailabilitiesByPhotographerIdAsync(int photographerId)
        {
            var availabilities = await _context.Availabilities
                .Where(a => a.PhotographerId == photographerId)
                .ToListAsync();

            _context.Availabilities.RemoveRange(availabilities);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
} 