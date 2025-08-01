using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Repository.Entity;
using SnapLink_Repository.Repository;
using SnapLink_Service.IService;

namespace SnapLink_Service.Service
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AvailabilityService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AvailabilityResponse>> GetAvailabilitiesByPhotographerIdAsync(int photographerId)
        {
            var availabilities = await _unitOfWork.AvailabilityRepository.GetAsync(
                filter: a => a.PhotographerId == photographerId,
                orderBy: q => q.OrderBy(a => a.DayOfWeek).ThenBy(a => a.StartTime),
                includeProperties: "Photographer");
            return _mapper.Map<IEnumerable<AvailabilityResponse>>(availabilities);
        }

        public async Task<IEnumerable<AvailabilityResponse>> GetAvailabilitiesByDayOfWeekAsync(DayOfWeek dayOfWeek)
        {
            var availabilities = await _unitOfWork.AvailabilityRepository.GetAsync(
                filter: a => a.DayOfWeek == dayOfWeek && a.Status == "Available",
                orderBy: q => q.OrderBy(a => a.StartTime),
                includeProperties: "Photographer");
            return _mapper.Map<IEnumerable<AvailabilityResponse>>(availabilities);
        }

        public async Task<AvailabilityResponse?> GetAvailabilityByIdAsync(int availabilityId)
        {
            var availabilities = await _unitOfWork.AvailabilityRepository.GetAsync(
                filter: a => a.AvailabilityId == availabilityId,
                includeProperties: "Photographer");
            var availability = availabilities.FirstOrDefault();
            return _mapper.Map<AvailabilityResponse>(availability);
        }

        public async Task<PhotographerAvailabilityResponse> GetPhotographerAvailabilityAsync(int photographerId)
        {
            var availabilities = await _unitOfWork.AvailabilityRepository.GetAsync(
                filter: a => a.PhotographerId == photographerId,
                orderBy: q => q.OrderBy(a => a.DayOfWeek).ThenBy(a => a.StartTime),
                includeProperties: "Photographer");
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
            var isAvailable = await IsTimeSlotAvailableAsync(
                request.PhotographerId, request.DayOfWeek, request.StartTime, request.EndTime);

            if (!isAvailable)
                throw new InvalidOperationException("Time slot conflicts with existing availability");

            var availability = _mapper.Map<Availability>(request);
            availability.CreatedAt = DateTime.UtcNow;
            await _unitOfWork.AvailabilityRepository.AddAsync(availability);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<AvailabilityResponse>(availability);
        }

        public async Task<AvailabilityResponse> UpdateAvailabilityAsync(int availabilityId, UpdateAvailabilityRequest request)
        {
            var availability = await _unitOfWork.AvailabilityRepository.GetByIdAsync(availabilityId);
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

            availability.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.AvailabilityRepository.Update(availability);
            await _unitOfWork.SaveChangesAsync();

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
            var availability = await _unitOfWork.AvailabilityRepository.GetByIdAsync(availabilityId);
            if (availability == null)
                return false;

            _unitOfWork.AvailabilityRepository.Remove(availability);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAvailabilitiesByPhotographerIdAsync(int photographerId)
        {
            var availabilities = await _unitOfWork.AvailabilityRepository.GetAsync(
                filter: a => a.PhotographerId == photographerId);
            
            if (availabilities.Any())
            {
                _unitOfWork.AvailabilityRepository.RemoveRange(availabilities);
                await _unitOfWork.SaveChangesAsync();
            }
            return true;
        }

        public async Task<AvailabilityCheckResponse> CheckAvailabilityAsync(CheckAvailabilityRequest request)
        {
            // Validate input first
            if (request.StartTime >= request.EndTime)
            {
                return new AvailabilityCheckResponse
                {
                    PhotographerId = request.PhotographerId,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    IsAvailable = false,
                    Message = "Invalid booking duration: start time must be before end time",
                    ConflictingAvailabilities = new List<AvailabilityResponse>()
                };
            }

            // Use the same logic as IsPhotographerAvailableAsync
            var isAvailable = await IsPhotographerAvailableAsync(
                request.PhotographerId, request.StartTime, request.EndTime);

            // For conflicting availabilities, we'll check the first day only
            // (This could be enhanced to show conflicts for all days in multi-day bookings)
            var dayOfWeek = request.StartTime.DayOfWeek;
            var startTime = request.StartTime.TimeOfDay;
            var endTime = request.EndTime.TimeOfDay;

            var conflictingAvailabilities = await GetConflictingAvailabilitiesAsync(
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
            // Validate input - ensure booking has a reasonable duration
            if (startTime >= endTime)
            {
                return false; // Invalid booking duration
            }

            // Check if start and end time are on the same day
            if (startTime.Date != endTime.Date)
            {
                // For multi-day bookings, check each day separately
                var currentDate = startTime.Date;
                var endDate = endTime.Date;
                
                while (currentDate <= endDate)
                {
                    var dayStart = currentDate == startTime.Date ? startTime : currentDate.Date;
                    var dayEnd = currentDate == endTime.Date ? endTime : currentDate.Date.AddDays(1);
                    
                    var dayOfWeek = currentDate.DayOfWeek;
                    var startTimeOfDay = dayStart.TimeOfDay;
                    var endTimeOfDay = dayEnd.TimeOfDay;
                    
                    var isAvailableForDay = await IsTimeSlotAvailableAsync(
                        photographerId, dayOfWeek, startTimeOfDay, endTimeOfDay);
                    
                    if (!isAvailableForDay)
                        return false;
                    
                    currentDate = currentDate.AddDays(1);
                }
                
                return true;
            }
            
            // Single day booking
            var singleDayOfWeek = startTime.DayOfWeek;
            var singleStartTimeOfDay = startTime.TimeOfDay;
            var singleEndTimeOfDay = endTime.TimeOfDay;

            return await IsTimeSlotAvailableAsync(
                photographerId, singleDayOfWeek, singleStartTimeOfDay, singleEndTimeOfDay);
        }

        private async Task<bool> IsTimeSlotAvailableAsync(int photographerId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            // Validate input
            if (startTime >= endTime)
                return false;

            // Check if there's any available slot that covers the requested time
            var availableSlots = await _unitOfWork.AvailabilityRepository.GetAsync(
                filter: a => a.PhotographerId == photographerId &&
                           a.DayOfWeek == dayOfWeek &&
                           a.Status == "Available" &&
                           a.StartTime <= startTime &&
                           a.EndTime >= endTime);

            return availableSlots.Any();
        }

        private async Task<IEnumerable<Availability>> GetConflictingAvailabilitiesAsync(int photographerId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            // Validate input
            if (startTime >= endTime)
                return new List<Availability>();

            // Find all available slots that overlap with the requested time
            return await _unitOfWork.AvailabilityRepository.GetAsync(
                filter: a => a.PhotographerId == photographerId &&
                           a.DayOfWeek == dayOfWeek &&
                           a.Status == "Available" &&
                           ((a.StartTime <= startTime && a.EndTime > startTime) ||
                            (a.StartTime < endTime && a.EndTime >= endTime) ||
                            (a.StartTime >= startTime && a.EndTime <= endTime)));
        }

        public async Task<bool> UpdateAvailabilityStatusAsync(int availabilityId, string status)
        {
            var availabilities = await _unitOfWork.AvailabilityRepository.GetAsync(
                filter: a => a.AvailabilityId == availabilityId);
            var availability = availabilities.FirstOrDefault();
            
            if (availability == null)
                return false;

            availability.Status = status;
            availability.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.AvailabilityRepository.Update(availability);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AvailabilityResponse>> GetAvailablePhotographersByTimeAsync(DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            var availabilities = await _unitOfWork.AvailabilityRepository.GetAsync(
                filter: a => a.DayOfWeek == dayOfWeek && 
                           a.Status == "Available" &&
                           a.StartTime <= startTime && 
                           a.EndTime >= endTime,
                includeProperties: "Photographer");
            
            return _mapper.Map<IEnumerable<AvailabilityResponse>>(availabilities);
        }
    }
} 