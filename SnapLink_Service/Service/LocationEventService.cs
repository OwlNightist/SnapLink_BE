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
    public class LocationEventService : ILocationEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBookingService _bookingService;

        public LocationEventService(IUnitOfWork unitOfWork, IMapper mapper, IBookingService bookingService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _bookingService = bookingService;
        }

        // Event Management
        public async Task<LocationEventResponse> CreateEventAsync(CreateLocationEventRequest request)
        {
            // Validate location exists
            var location = await _unitOfWork.LocationRepository.GetByIdAsync(request.LocationId);
            if (location == null)
                throw new ArgumentException($"Location with ID {request.LocationId} not found");

            // Validate date range
            if (request.StartDate >= request.EndDate)
                throw new ArgumentException("Start date must be before end date");

            if (request.StartDate < DateTime.UtcNow)
                throw new ArgumentException("Start date cannot be in the past");

            var locationEvent = _mapper.Map<LocationEvent>(request);
            locationEvent.Status = "Draft";
            locationEvent.CreatedAt = DateTime.UtcNow;
            locationEvent.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.LocationEventRepository.AddAsync(locationEvent);
            await _unitOfWork.SaveChangesAsync();

            return await GetEventByIdAsync(locationEvent.EventId);
        }

        public async Task<LocationEventResponse> GetEventByIdAsync(int eventId)
        {
            var locationEvent = await _unitOfWork.LocationEventRepository.GetAsync(
                filter: e => e.EventId == eventId,
                includeProperties: "Location,Images"
            );

            var eventEntity = locationEvent.FirstOrDefault();
            if (eventEntity == null)
                throw new ArgumentException($"Event with ID {eventId} not found");

            var response = _mapper.Map<LocationEventResponse>(eventEntity);
            
            // Get counts
            var approvedPhotographers = await _unitOfWork.EventPhotographerRepository.GetAsync(
                filter: ep => ep.EventId == eventId && ep.Status == "Approved"
            );
            response.ApprovedPhotographersCount = approvedPhotographers.Count();

            var totalBookings = await _unitOfWork.EventBookingRepository.GetAsync(
                filter: eb => eb.EventId == eventId
            );
            response.TotalBookingsCount = totalBookings.Count();

            return response;
        }

        public async Task<LocationEventDetailResponse> GetEventDetailAsync(int eventId)
        {
            var locationEvent = await _unitOfWork.LocationEventRepository.GetAsync(
                filter: e => e.EventId == eventId,
                includeProperties: "Location,Images,EventPhotographers.Photographer.User,EventBookings.Booking.User"
            );

            var eventEntity = locationEvent.FirstOrDefault();
            if (eventEntity == null)
                throw new ArgumentException($"Event with ID {eventId} not found");

            return _mapper.Map<LocationEventDetailResponse>(eventEntity);
        }

        public async Task<IEnumerable<LocationEventResponse>> GetAllEventsAsync()
        {
            var events = await _unitOfWork.LocationEventRepository.GetAsync(
                includeProperties: "Location,Images"
            );
            return _mapper.Map<IEnumerable<LocationEventResponse>>(events);
        }

        public async Task<IEnumerable<LocationEventResponse>> GetEventsByLocationAsync(int locationId)
        {
            var events = await _unitOfWork.LocationEventRepository.GetAsync(
                filter: e => e.LocationId == locationId,
                includeProperties: "Location,Images"
            );
            return _mapper.Map<IEnumerable<LocationEventResponse>>(events);
        }

        public async Task<IEnumerable<LocationEventResponse>> GetActiveEventsAsync()
        {
            var now = DateTime.UtcNow;
            var events = await _unitOfWork.LocationEventRepository.GetAsync(
                filter: e => e.Status == "Active" && e.StartDate <= now && e.EndDate >= now,
                includeProperties: "Location,Images"
            );
            return _mapper.Map<IEnumerable<LocationEventResponse>>(events);
        }

        public async Task<IEnumerable<LocationEventResponse>> GetUpcomingEventsAsync()
        {
            var now = DateTime.UtcNow;
            var events = await _unitOfWork.LocationEventRepository.GetAsync(
                filter: e => e.Status == "Open" && e.StartDate > now,
                includeProperties: "Location"
            );
            return _mapper.Map<IEnumerable<LocationEventResponse>>(events);
        }

        public async Task<LocationEventResponse> UpdateEventAsync(int eventId, UpdateLocationEventRequest request)
        {
            var locationEvent = await _unitOfWork.LocationEventRepository.GetByIdAsync(eventId);
            if (locationEvent == null)
                throw new ArgumentException($"Event with ID {eventId} not found");

            // Update only provided fields
            if (!string.IsNullOrEmpty(request.Name))
                locationEvent.Name = request.Name;
            if (!string.IsNullOrEmpty(request.Description))
                locationEvent.Description = request.Description;
            if (request.StartDate.HasValue)
                locationEvent.StartDate = request.StartDate.Value;
            if (request.EndDate.HasValue)
                locationEvent.EndDate = request.EndDate.Value;
            if (request.DiscountedPrice.HasValue)
                locationEvent.DiscountedPrice = request.DiscountedPrice.Value;
            if (request.OriginalPrice.HasValue)
                locationEvent.OriginalPrice = request.OriginalPrice.Value;
            if (request.MaxPhotographers.HasValue)
                locationEvent.MaxPhotographers = request.MaxPhotographers.Value;
            if (request.MaxBookingsPerSlot.HasValue)
                locationEvent.MaxBookingsPerSlot = request.MaxBookingsPerSlot.Value;
            if (!string.IsNullOrEmpty(request.Status))
                locationEvent.Status = request.Status;

            locationEvent.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.LocationEventRepository.Update(locationEvent);
            await _unitOfWork.SaveChangesAsync();

            return await GetEventByIdAsync(eventId);
        }

        public async Task<bool> DeleteEventAsync(int eventId)
        {
            var locationEvent = await _unitOfWork.LocationEventRepository.GetByIdAsync(eventId);
            if (locationEvent == null)
                return false;

            _unitOfWork.LocationEventRepository.Remove(locationEvent);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateEventStatusAsync(int eventId, string status)
        {
            var locationEvent = await _unitOfWork.LocationEventRepository.GetByIdAsync(eventId);
            if (locationEvent == null)
                return false;

            locationEvent.Status = status;
            locationEvent.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.LocationEventRepository.Update(locationEvent);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // Event Applications
        public async Task<EventApplicationResponse> ApplyToEventAsync(EventApplicationRequest request)
        {
            // Validate event exists and is open for applications
            var locationEvent = await _unitOfWork.LocationEventRepository.GetByIdAsync(request.EventId);
            if (locationEvent == null)
                throw new ArgumentException($"Event with ID {request.EventId} not found");

            if (locationEvent.Status != "Open")
                throw new InvalidOperationException("Event is not open for applications");

            // Validate photographer exists
            var photographer = await _unitOfWork.PhotographerRepository.GetByIdAsync(request.PhotographerId);
            if (photographer == null)
                throw new ArgumentException($"Photographer with ID {request.PhotographerId} not found");

            // Check if already applied
            var existingApplication = await _unitOfWork.EventPhotographerRepository.GetAsync(
                filter: ep => ep.EventId == request.EventId && ep.PhotographerId == request.PhotographerId
            );
            if (existingApplication.Any())
                throw new InvalidOperationException("Photographer has already applied to this event");

            // Check capacity
            var approvedCount = await _unitOfWork.EventPhotographerRepository.GetAsync(
                filter: ep => ep.EventId == request.EventId && ep.Status == "Approved"
            );
            if (approvedCount.Count() >= locationEvent.MaxPhotographers)
                throw new InvalidOperationException("Event has reached maximum photographer capacity");

            var eventPhotographer = new EventPhotographer
            {
                EventId = request.EventId,
                PhotographerId = request.PhotographerId,
                Status = "Applied",
                AppliedAt = DateTime.UtcNow,
                SpecialRate = request.SpecialRate
            };

            await _unitOfWork.EventPhotographerRepository.AddAsync(eventPhotographer);
            await _unitOfWork.SaveChangesAsync();

            return await GetEventApplicationResponseAsync(eventPhotographer.EventPhotographerId);
        }

        public async Task<EventApplicationResponse> RespondToApplicationAsync(EventApplicationResponseRequest request)
        {
            var eventPhotographer = await _unitOfWork.EventPhotographerRepository.GetAsync(
                filter: ep => ep.EventId == request.EventId && ep.PhotographerId == request.PhotographerId
            );

            var application = eventPhotographer.FirstOrDefault();
            if (application == null)
                throw new ArgumentException("Application not found");

            if (application.Status != "Applied")
                throw new InvalidOperationException("Application has already been processed");

            application.Status = request.Status;
            if (request.Status == "Approved")
            {
                application.ApprovedAt = DateTime.UtcNow;
            }
            else if (request.Status == "Rejected")
            {
                application.RejectionReason = request.RejectionReason;
            }

            _unitOfWork.EventPhotographerRepository.Update(application);
            await _unitOfWork.SaveChangesAsync();

            return await GetEventApplicationResponseAsync(application.EventPhotographerId);
        }

        public async Task<IEnumerable<EventApplicationResponse>> GetEventApplicationsAsync(int eventId)
        {
            var applications = await _unitOfWork.EventPhotographerRepository.GetAsync(
                filter: ep => ep.EventId == eventId,
                includeProperties: "Photographer.User,Event"
            );

            var responses = new List<EventApplicationResponse>();
            foreach (var app in applications)
            {
                responses.Add(await GetEventApplicationResponseAsync(app.EventPhotographerId));
            }

            return responses;
        }

        public async Task<IEnumerable<EventApplicationResponse>> GetPhotographerApplicationsAsync(int photographerId)
        {
            var applications = await _unitOfWork.EventPhotographerRepository.GetAsync(
                filter: ep => ep.PhotographerId == photographerId,
                includeProperties: "Event,Photographer.User"
            );

            var responses = new List<EventApplicationResponse>();
            foreach (var app in applications)
            {
                responses.Add(await GetEventApplicationResponseAsync(app.EventPhotographerId));
            }

            return responses;
        }

        public async Task<bool> WithdrawApplicationAsync(int eventId, int photographerId)
        {
            var eventPhotographer = await _unitOfWork.EventPhotographerRepository.GetAsync(
                filter: ep => ep.EventId == eventId && ep.PhotographerId == photographerId
            );

            var application = eventPhotographer.FirstOrDefault();
            if (application == null)
                return false;

            if (application.Status != "Applied")
                return false; // Can only withdraw pending applications

            application.Status = "Withdrawn";
            _unitOfWork.EventPhotographerRepository.Update(application);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EventPhotographerResponse>> GetApprovedPhotographersAsync(int eventId)
        {
            var approvedPhotographers = await _unitOfWork.EventPhotographerRepository.GetAsync(
                filter: ep => ep.EventId == eventId && ep.Status == "Approved",
                includeProperties: "Photographer.User,Event"
            );

            return _mapper.Map<IEnumerable<EventPhotographerResponse>>(approvedPhotographers);
        }

        // Event Bookings
        public async Task<EventBookingResponse> CreateEventBookingAsync(EventBookingRequest request)
        {
            // Validate event exists and is active
            var locationEventEntity = await _unitOfWork.LocationEventRepository.GetByIdAsync(request.EventId);
            if (locationEventEntity == null)
                throw new ArgumentException($"Event with ID {request.EventId} not found");

            if (locationEventEntity.Status != "Active" && locationEventEntity.Status != "Open")
                throw new InvalidOperationException("Event is not active or open for bookings");

            // Validate event photographer exists and is approved
            var eventPhotographer = await _unitOfWork.EventPhotographerRepository.GetAsync(
                filter: ep => ep.EventPhotographerId == request.EventPhotographerId && ep.EventId == request.EventId
            );
            var approvedPhotographer = eventPhotographer.FirstOrDefault();
            if (approvedPhotographer == null || approvedPhotographer.Status != "Approved")
                throw new ArgumentException("Invalid event photographer or not approved");

            // Create a CreateBookingRequest to use BookingService validation
            var createBookingRequest = new CreateBookingRequest
            {
                PhotographerId = approvedPhotographer.PhotographerId,
                LocationId = locationEventEntity.LocationId,
                StartDatetime = request.StartDatetime,
                EndDatetime = request.EndDatetime,
                SpecialRequests = request.SpecialRequests
            };

            // Use BookingService validation
            var validationResult = await _bookingService.ValidateBookingRequestAsync(createBookingRequest, request.UserId);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            // Validate user exists
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
            if (user == null)
                throw new ArgumentException($"User with ID {request.UserId} not found");
            var duration = (request.EndDatetime - request.StartDatetime).TotalHours;
            // Calculate price using event pricing or fallback to regular calculation
            decimal totalPrice = (approvedPhotographer.SpecialRate.Value + locationEventEntity.DiscountedPrice.Value) * (decimal)duration;


            // Create regular booking first
            var booking = new Booking
            {
                UserId = request.UserId,
                PhotographerId = approvedPhotographer.PhotographerId,
                LocationId = locationEventEntity.LocationId,
                StartDatetime = request.StartDatetime,
                SpecialRequests = request.SpecialRequests,
                EndDatetime = request.EndDatetime,
                Status = "Pending",
                TotalPrice = totalPrice,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.BookingRepository.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            // Create event booking
            var eventBooking = new EventBooking
            {
                EventId = request.EventId,
                BookingId = booking.BookingId,
                EventPhotographerId = request.EventPhotographerId,
                EventPrice = totalPrice,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.EventBookingRepository.AddAsync(eventBooking);
            await _unitOfWork.SaveChangesAsync();

            return await GetEventBookingByIdAsync(eventBooking.EventBookingId);
        }

        public async Task<EventBookingResponse> GetEventBookingByIdAsync(int eventBookingId)
        {
            var eventBooking = await _unitOfWork.EventBookingRepository.GetAsync(
                filter: eb => eb.EventBookingId == eventBookingId,
                includeProperties: "Booking.User,EventPhotographer.Photographer.User,Event"
            );

            var bookingEntity = eventBooking.FirstOrDefault();
            if (bookingEntity == null)
                throw new ArgumentException($"Event booking with ID {eventBookingId} not found");

            return _mapper.Map<EventBookingResponse>(bookingEntity);
        }

        public async Task<IEnumerable<EventBookingResponse>> GetEventBookingsAsync(int eventId)
        {
            var eventBookings = await _unitOfWork.EventBookingRepository.GetAsync(
                filter: eb => eb.EventId == eventId,
                includeProperties: "Booking.User,EventPhotographer.Photographer.User,Event"
            );

            return _mapper.Map<IEnumerable<EventBookingResponse>>(eventBookings);
        }

        public async Task<IEnumerable<EventBookingResponse>> GetUserEventBookingsAsync(int userId)
        {
            var eventBookings = await _unitOfWork.EventBookingRepository.GetAsync(
                filter: eb => eb.Booking.UserId == userId,
                includeProperties: "Booking.User,EventPhotographer.Photographer.User,Event"
            );

            return _mapper.Map<IEnumerable<EventBookingResponse>>(eventBookings);
        }

        public async Task<bool> CancelEventBookingAsync(int eventBookingId)
        {
            var eventBooking = await _unitOfWork.EventBookingRepository.GetAsync(
                filter: eb => eb.EventBookingId == eventBookingId
            );

            var bookingEntity = eventBooking.FirstOrDefault();
            if (bookingEntity == null)
                return false;

            // Update booking status
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingEntity.BookingId);
            if (booking != null)
            {
                booking.Status = "Cancelled";
                booking.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.BookingRepository.Update(booking);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // Event Statistics
        public async Task<EventStatisticsResponse> GetEventStatisticsAsync(int eventId)
        {
            var locationEvent = await _unitOfWork.LocationEventRepository.GetByIdAsync(eventId);
            if (locationEvent == null)
                throw new ArgumentException($"Event with ID {eventId} not found");

            var applications = await _unitOfWork.EventPhotographerRepository.GetAsync(
                filter: ep => ep.EventId == eventId
            );

            var bookings = await _unitOfWork.EventBookingRepository.GetAsync(
                filter: eb => eb.EventId == eventId
            );

            var statistics = new EventStatisticsResponse
            {
                EventId = eventId,
                EventName = locationEvent.Name,
                EventStartDate = locationEvent.StartDate,
                EventEndDate = locationEvent.EndDate,
                EventStatus = locationEvent.Status,
                TotalApplications = applications.Count(),
                ApprovedApplications = applications.Count(a => a.Status == "Approved"),
                RejectedApplications = applications.Count(a => a.Status == "Rejected"),
                PendingApplications = applications.Count(a => a.Status == "Applied"),
                TotalBookings = bookings.Count(),
                TotalRevenue = bookings.Sum(b => b.EventPrice),
                AverageBookingValue = bookings.Any() ? bookings.Average(b => b.EventPrice) : 0
            };

            return statistics;
        }

        public async Task<IEnumerable<EventListResponse>> GetEventListAsync()
        {
            var events = await _unitOfWork.LocationEventRepository.GetAsync(
                includeProperties: "Location"
            );

            var responses = new List<EventListResponse>();
            foreach (var evt in events)
            {
                var response = _mapper.Map<EventListResponse>(evt);
                
                // Get counts
                var approvedPhotographers = await _unitOfWork.EventPhotographerRepository.GetAsync(
                    filter: ep => ep.EventId == evt.EventId && ep.Status == "Approved"
                );
                response.ApprovedPhotographersCount = approvedPhotographers.Count();

                var totalBookings = await _unitOfWork.EventBookingRepository.GetAsync(
                    filter: eb => eb.EventId == evt.EventId
                );
                response.TotalBookingsCount = totalBookings.Count();

                responses.Add(response);
            }

            return responses;
        }

        public async Task<IEnumerable<EventListResponse>> GetEventsByStatusAsync(string status)
        {
            var events = await _unitOfWork.LocationEventRepository.GetAsync(
                filter: e => e.Status == status,
                includeProperties: "Location"
            );

            return await MapToEventListResponses(events);
        }

        public async Task<IEnumerable<EventListResponse>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var events = await _unitOfWork.LocationEventRepository.GetAsync(
                filter: e => e.StartDate >= startDate && e.EndDate <= endDate,
                includeProperties: "Location"
            );

            return await MapToEventListResponses(events);
        }

        // Event Discovery
        public async Task<IEnumerable<EventListResponse>> SearchEventsAsync(string searchTerm)
        {
            var events = await _unitOfWork.LocationEventRepository.GetAsync(
                filter: e => e.Name.Contains(searchTerm) || e.Description.Contains(searchTerm),
                includeProperties: "Location"
            );

            return await MapToEventListResponses(events);
        }

        public async Task<IEnumerable<EventListResponse>> GetFeaturedEventsAsync()
        {
            var events = await _unitOfWork.LocationEventRepository.GetAsync(
                filter: e => e.Status == "Active" || e.Status == "Open",
                includeProperties: "Location"
            );

            // For now, return all active/open events. Could be enhanced with featured flag
            return await MapToEventListResponses(events);
        }

        public async Task<IEnumerable<EventListResponse>> GetEventsNearbyAsync(double latitude, double longitude, double radiusKm)
        {
            // This would require location coordinates in the Location entity
            // For now, return all events. Could be enhanced with distance calculation
            var events = await _unitOfWork.LocationEventRepository.GetAsync(
                filter: e => e.Status == "Active" || e.Status == "Open",
                includeProperties: "Location"
            );

            return await MapToEventListResponses(events);
        }

        // Helper methods
        private async Task<EventApplicationResponse> GetEventApplicationResponseAsync(int eventPhotographerId)
        {
            var eventPhotographer = await _unitOfWork.EventPhotographerRepository.GetAsync(
                filter: ep => ep.EventPhotographerId == eventPhotographerId,
                includeProperties: "Event,Photographer.User"
            );

            var app = eventPhotographer.FirstOrDefault();
            if (app == null)
                throw new ArgumentException("Application not found");

            return new EventApplicationResponse
            {
                EventPhotographerId = app.EventPhotographerId,
                EventId = app.EventId,
                PhotographerId = app.PhotographerId,
                Status = app.Status,
                AppliedAt = app.AppliedAt,
                ApprovedAt = app.ApprovedAt,
                RejectionReason = app.RejectionReason,
                SpecialRate = app.SpecialRate,
                EventName = app.Event.Name,
                EventStartDate = app.Event.StartDate,
                EventEndDate = app.Event.EndDate,
                EventStatus = app.Event.Status,
                PhotographerName = app.Photographer.User.FullName ?? app.Photographer.User.UserName ?? "Unknown",
                PhotographerProfileImage = app.Photographer.User.ProfileImage,
                PhotographerRating = app.Photographer.Rating
            };
        }

        private async Task<IEnumerable<EventListResponse>> MapToEventListResponses(IEnumerable<LocationEvent> events)
        {
            var responses = new List<EventListResponse>();
            foreach (var evt in events)
            {
                var response = _mapper.Map<EventListResponse>(evt);
                
                // Get counts
                var approvedPhotographers = await _unitOfWork.EventPhotographerRepository.GetAsync(
                    filter: ep => ep.EventId == evt.EventId && ep.Status == "Approved"
                );
                response.ApprovedPhotographersCount = approvedPhotographers.Count();

                var totalBookings = await _unitOfWork.EventBookingRepository.GetAsync(
                    filter: eb => eb.EventId == evt.EventId
                );
                response.TotalBookingsCount = totalBookings.Count();

                responses.Add(response);
            }

            return responses;
        }
    }
}
