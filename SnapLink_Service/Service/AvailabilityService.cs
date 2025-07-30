using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Repository.Entity;
using SnapLink_Repository.IRepository;
using SnapLink_Service.IService;

namespace SnapLink_Service.Service
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly IMapper _mapper;

        public AvailabilityService(IAvailabilityRepository availabilityRepository, IMapper mapper)
        {
            _availabilityRepository = availabilityRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AvailabilityResponse>> GetAvailabilitiesByPhotographerIdAsync(int photographerId)
        {
            var availabilities = await _availabilityRepository.GetAvailabilitiesByPhotographerIdAsync(photographerId);
            return _mapper.Map<IEnumerable<AvailabilityResponse>>(availabilities);
        }

        public async Task<IEnumerable<AvailabilityResponse>> GetAvailabilitiesByDayOfWeekAsync(DayOfWeek dayOfWeek)
        {
            var availabilities = await _availabilityRepository.GetAvailabilitiesByDayOfWeekAsync(dayOfWeek);
            return _mapper.Map<IEnumerable<AvailabilityResponse>>(availabilities);
        }

        public async Task<AvailabilityResponse?> GetAvailabilityByIdAsync(int availabilityId)
        {
            var availability = await _availabilityRepository.GetAvailabilityByIdAsync(availabilityId);
            return _mapper.Map<AvailabilityResponse>(availability);
        }

        public async Task<PhotographerAvailabilityResponse> GetPhotographerAvailabilityAsync(int photographerId)
        {
            var availabilities = await _availabilityRepository.GetAvailabilitiesByPhotographerIdAsync(photographerId);
            var availabilityResponses = _mapper.Map<IEnumerable<AvailabilityResponse>>(availabilities);

            return new PhotographerAvailabilityResponse
            {
                PhotographerId = photographerId,
                Availabilities = availabilityResponses.ToList()
            };
        }

        public async Task<AvailabilityResponse> CreateAvailabilityAsync(CreateAvailabilityRequest request)
        {
            // Validate time range
            if (request.StartTime >= request.EndTime)
                throw new ArgumentException("Start time must be before end time");

            // Check for conflicts
            var isAvailable = await _availabilityRepository.IsTimeSlotAvailableAsync(
                request.PhotographerId, request.DayOfWeek, request.StartTime, request.EndTime);

            if (!isAvailable)
                throw new InvalidOperationException("Time slot conflicts with existing availability");

            var availability = _mapper.Map<Availability>(request);
            await _availabilityRepository.AddAvailabilityAsync(availability);
            await _availabilityRepository.SaveChangesAsync();

            return _mapper.Map<AvailabilityResponse>(availability);
        }

        public async Task<AvailabilityResponse> UpdateAvailabilityAsync(int availabilityId, UpdateAvailabilityRequest request)
        {
            var availability = await _availabilityRepository.GetAvailabilityByIdAsync(availabilityId);
            if (availability == null)
                throw new ArgumentException($"Availability with ID {availabilityId} not found");

            // Update properties if provided
            if (request.DayOfWeek.HasValue)
                availability.DayOfWeek = request.DayOfWeek.Value;
            if (request.StartTime.HasValue)
                availability.StartTime = request.StartTime.Value;
            if (request.EndTime.HasValue)
                availability.EndTime = request.EndTime.Value;
            if (!string.IsNullOrEmpty(request.Status))
                availability.Status = request.Status;

            // Validate time range if both times are provided
            if (request.StartTime.HasValue && request.EndTime.HasValue && request.StartTime >= request.EndTime)
                throw new ArgumentException("Start time must be before end time");

            await _availabilityRepository.UpdateAvailabilityAsync(availability);
            await _availabilityRepository.SaveChangesAsync();

            return _mapper.Map<AvailabilityResponse>(availability);
        }

        public async Task<bool> CreateBulkAvailabilityAsync(BulkAvailabilityRequest request)
        {
            try
            {
                foreach (var availabilityRequest in request.Availabilities)
                {
                    availabilityRequest.PhotographerId = request.PhotographerId;
                    await CreateAvailabilityAsync(availabilityRequest);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAvailabilityAsync(int availabilityId)
        {
            var availability = await _availabilityRepository.GetAvailabilityByIdAsync(availabilityId);
            if (availability == null)
                return false;

            await _availabilityRepository.DeleteAvailabilityAsync(availability);
            await _availabilityRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAvailabilitiesByPhotographerIdAsync(int photographerId)
        {
            await _availabilityRepository.DeleteAvailabilitiesByPhotographerIdAsync(photographerId);
            await _availabilityRepository.SaveChangesAsync();
            return true;
        }

        public async Task<AvailabilityCheckResponse> CheckAvailabilityAsync(CheckAvailabilityRequest request)
        {
            var dayOfWeek = request.StartTime.DayOfWeek;
            var startTime = request.StartTime.TimeOfDay;
            var endTime = request.EndTime.TimeOfDay;

            var isAvailable = await _availabilityRepository.IsTimeSlotAvailableAsync(
                request.PhotographerId, dayOfWeek, startTime, endTime);

            var conflictingAvailabilities = await _availabilityRepository.GetConflictingAvailabilitiesAsync(
                request.PhotographerId, dayOfWeek, startTime, endTime);

            return new AvailabilityCheckResponse
            {
                PhotographerId = request.PhotographerId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                IsAvailable = isAvailable,
                Message = isAvailable ? "Time slot is available" : "Time slot conflicts with existing availability",
                ConflictingAvailabilities = _mapper.Map<IEnumerable<AvailabilityResponse>>(conflictingAvailabilities).ToList()
            };
        }

        public async Task<bool> IsPhotographerAvailableAsync(int photographerId, DateTime startTime, DateTime endTime)
        {
            var dayOfWeek = startTime.DayOfWeek;
            var startTimeOfDay = startTime.TimeOfDay;
            var endTimeOfDay = endTime.TimeOfDay;

            return await _availabilityRepository.IsTimeSlotAvailableAsync(
                photographerId, dayOfWeek, startTimeOfDay, endTimeOfDay);
        }

        public async Task<bool> UpdateAvailabilityStatusAsync(int availabilityId, string status)
        {
            var availability = await _availabilityRepository.GetAvailabilityByIdAsync(availabilityId);
            if (availability == null)
                return false;

            availability.Status = status;
            await _availabilityRepository.UpdateAvailabilityAsync(availability);
            await _availabilityRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AvailabilityResponse>> GetAvailablePhotographersByTimeAsync(DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            var availabilities = await _availabilityRepository.GetAvailabilitiesByDayOfWeekAsync(dayOfWeek);
            
            var availableSlots = availabilities.Where(a => 
                a.Status == "Available" &&
                a.StartTime <= startTime && 
                a.EndTime >= endTime);

            return _mapper.Map<IEnumerable<AvailabilityResponse>>(availableSlots);
        }
    }
} 