using SnapLink_Repository.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SnapLink_Repository.IRepository
{
    public interface IAvailabilityRepository
    {
        Task<IEnumerable<Availability>> GetAvailabilitiesByPhotographerIdAsync(int photographerId);
        Task<IEnumerable<Availability>> GetAvailabilitiesByDayOfWeekAsync(DayOfWeek dayOfWeek);
        Task<IEnumerable<Availability>> GetAvailableSlotsAsync(int photographerId, DayOfWeek dayOfWeek);
        Task<bool> IsTimeSlotAvailableAsync(int photographerId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime);
        Task<IEnumerable<Availability>> GetConflictingAvailabilitiesAsync(int photographerId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime);
        Task<IEnumerable<Availability>> GetAvailabilitiesByStatusAsync(string status);
        Task<Availability?> GetAvailabilityByIdAsync(int availabilityId);
        Task AddAvailabilityAsync(Availability availability);
        Task UpdateAvailabilityAsync(Availability availability);
        Task DeleteAvailabilityAsync(Availability availability);
        Task DeleteAvailabilitiesByPhotographerIdAsync(int photographerId);
        Task SaveChangesAsync();
    }
} 