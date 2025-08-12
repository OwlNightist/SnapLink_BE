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

            // Check for conflicts with existing availabilities
            var hasConflicts = await HasTimeSlotConflictsAsync(
                request.PhotographerId, request.DayOfWeek, request.StartTime, request.EndTime);

            if (hasConflicts)
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

        private async Task<bool> HasTimeSlotConflictsAsync(int photographerId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            // Validate input
            if (startTime >= endTime)
                return false;

            // Check for any existing availabilities that overlap with the requested time slot
            var conflictingSlots = await _unitOfWork.AvailabilityRepository.GetAsync(
                filter: a => a.PhotographerId == photographerId &&
                           a.DayOfWeek == dayOfWeek &&
                           a.Status == "Available" &&
                           ((a.StartTime < endTime && a.EndTime > startTime)));

            return conflictingSlots.Any();
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

        public async Task<IEnumerable<AvailableTimeSlotResponse>> GetAvailableTimeSlotsAsync(int photographerId, DateTime date)
        {
            var dayOfWeek = date.DayOfWeek;
            var result = new AvailableTimeSlotResponse
            {
                PhotographerId = photographerId,
                Date = date.Date,
                DayOfWeek = dayOfWeek
            };

            // Get registered availability for this day
            var registeredAvailabilities = await _unitOfWork.AvailabilityRepository.GetAsync(
                filter: a => a.PhotographerId == photographerId && 
                           a.DayOfWeek == dayOfWeek && 
                           a.Status == "Available",
                orderBy: q => q.OrderBy(a => a.StartTime));

            if (!registeredAvailabilities.Any())
            {
                return new List<AvailableTimeSlotResponse> { result };
            }

            // Get existing bookings for this photographer on this date
            var existingBookings = await _unitOfWork.BookingRepository.GetAsync(
                filter: b => b.PhotographerId == photographerId &&
                           b.StartDatetime.HasValue &&
                           b.EndDatetime.HasValue &&
                           b.StartDatetime.Value.Date == date.Date &&
                           b.Status != "Cancelled" &&
                           b.Status != "Completed",
                orderBy: q => q.OrderBy(b => b.StartDatetime.Value),
                includeProperties: "User");

            // Process each registered availability slot
            foreach (var availability in registeredAvailabilities)
            {
                var registeredSlot = new TimeSlot
                {
                    StartTime = availability.StartTime,
                    EndTime = availability.EndTime,
                    Status = "Registered"
                };
                result.RegisteredAvailability.Add(registeredSlot);

                // Find overlapping bookings for this availability slot
                var overlappingBookings = existingBookings.Where(b => 
                    b.StartDatetime.HasValue &&
                    b.EndDatetime.HasValue &&
                    b.StartDatetime.Value.TimeOfDay < availability.EndTime &&
                    b.EndDatetime.Value.TimeOfDay > availability.StartTime).ToList();

                if (!overlappingBookings.Any())
                {
                    // No conflicts - entire slot is available
                    result.AvailableSlots.Add(new TimeSlot
                    {
                        StartTime = availability.StartTime,
                        EndTime = availability.EndTime,
                        Status = "Available"
                    });
                }
                else
                {
                    // Process conflicts and find available sub-slots
                    var availableSlots = CalculateAvailableSubSlots(
                        availability.StartTime, 
                        availability.EndTime, 
                        overlappingBookings);

                    foreach (var slot in availableSlots)
                    {
                        result.AvailableSlots.Add(slot);
                    }

                    // Add booked slots for display
                    foreach (var booking in overlappingBookings)
                    {
                        if (!booking.StartDatetime.HasValue || !booking.EndDatetime.HasValue)
                            continue;
                            
                        var bookedSlot = new TimeSlot
                        {
                            StartTime = TimeSpan.FromTicks(Math.Max(availability.StartTime.Ticks, booking.StartDatetime.Value.TimeOfDay.Ticks)),
                            EndTime = TimeSpan.FromTicks(Math.Min(availability.EndTime.Ticks, booking.EndDatetime.Value.TimeOfDay.Ticks)),
                            Status = "Booked",
                            BookingInfo = $"Booking #{booking.BookingId} - {booking.User?.FullName ?? "Unknown"}"
                        };
                        result.BookedSlots.Add(bookedSlot);
                    }
                }
            }

            return new List<AvailableTimeSlotResponse> { result };
        }

        private List<TimeSlot> CalculateAvailableSubSlots(TimeSpan availabilityStart, TimeSpan availabilityEnd, List<Booking> overlappingBookings)
        {
            var availableSlots = new List<TimeSlot>();
            var currentTime = availabilityStart;

            // Sort bookings by start time
            var sortedBookings = overlappingBookings
                .Where(b => b.StartDatetime.HasValue)
                .OrderBy(b => b.StartDatetime.Value.TimeOfDay)
                .ToList();

            foreach (var booking in sortedBookings)
            {
                if (!booking.StartDatetime.HasValue || !booking.EndDatetime.HasValue)
                    continue;
                    
                var bookingStart = booking.StartDatetime.Value.TimeOfDay;
                var bookingEnd = booking.EndDatetime.Value.TimeOfDay;

                // If there's a gap between current time and booking start, it's available
                if (currentTime < bookingStart)
                {
                    availableSlots.Add(new TimeSlot
                    {
                        StartTime = currentTime,
                        EndTime = bookingStart,
                        Status = "Available"
                    });
                }

                // Move current time to after this booking
                currentTime = TimeSpan.FromTicks(Math.Max(currentTime.Ticks, bookingEnd.Ticks));
            }

            // If there's remaining time after all bookings, it's available
            if (currentTime < availabilityEnd)
            {
                availableSlots.Add(new TimeSlot
                {
                    StartTime = currentTime,
                    EndTime = availabilityEnd,
                    Status = "Available"
                });
            }

            return availableSlots;
        }
    }
} 