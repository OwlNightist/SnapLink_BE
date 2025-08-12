using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SnapLink_Service.IService
{
    public interface IAvailabilityService
    {
        // Get availabilities
        Task<IEnumerable<AvailabilityResponse>> GetAvailabilitiesByPhotographerIdAsync(int photographerId);
        Task<IEnumerable<AvailabilityResponse>> GetAvailabilitiesByDayOfWeekAsync(DayOfWeek dayOfWeek);
        Task<AvailabilityResponse?> GetAvailabilityByIdAsync(int availabilityId);
        Task<PhotographerAvailabilityResponse> GetPhotographerAvailabilityAsync(int photographerId);

        // Create and update availabilities
        Task<AvailabilityResponse> CreateAvailabilityAsync(CreateAvailabilityRequest request);
        Task<AvailabilityResponse> UpdateAvailabilityAsync(int availabilityId, UpdateAvailabilityRequest request);
        Task<bool> CreateBulkAvailabilityAsync(BulkAvailabilityRequest request);

        // Delete availabilities
        Task<bool> DeleteAvailabilityAsync(int availabilityId);
        Task<bool> DeleteAvailabilitiesByPhotographerIdAsync(int photographerId);

        // Check availability
        Task<AvailabilityCheckResponse> CheckAvailabilityAsync(CheckAvailabilityRequest request);
        Task<bool> IsPhotographerAvailableAsync(int photographerId, DateTime startTime, DateTime endTime);

        // Business logic
        Task<bool> UpdateAvailabilityStatusAsync(int availabilityId, string status);
        Task<IEnumerable<AvailabilityResponse>> GetAvailablePhotographersByTimeAsync(DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime);
        
        /// <summary>
        /// Get available time slots for a photographer on a specific day, considering existing bookings
        /// </summary>
        Task<IEnumerable<AvailableTimeSlotResponse>> GetAvailableTimeSlotsAsync(int photographerId, DateTime date);
    }
} 