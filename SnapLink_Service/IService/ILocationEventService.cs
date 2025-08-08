using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService
{
    public interface ILocationEventService
    {
        // Event Management
        Task<LocationEventResponse> CreateEventAsync(CreateLocationEventRequest request);
        Task<LocationEventResponse> GetEventByIdAsync(int eventId);
        Task<LocationEventDetailResponse> GetEventDetailAsync(int eventId);
        Task<IEnumerable<LocationEventResponse>> GetAllEventsAsync();
        Task<IEnumerable<LocationEventResponse>> GetEventsByLocationAsync(int locationId);
        Task<IEnumerable<LocationEventResponse>> GetActiveEventsAsync();
        Task<IEnumerable<LocationEventResponse>> GetUpcomingEventsAsync();
        Task<LocationEventResponse> UpdateEventAsync(int eventId, UpdateLocationEventRequest request);
        Task<bool> DeleteEventAsync(int eventId);
        Task<bool> UpdateEventStatusAsync(int eventId, string status);

        // Event Applications
        Task<EventApplicationResponse> ApplyToEventAsync(EventApplicationRequest request);
        Task<EventApplicationResponse> RespondToApplicationAsync(EventApplicationResponseRequest request);
        Task<IEnumerable<EventApplicationResponse>> GetEventApplicationsAsync(int eventId);
        Task<IEnumerable<EventApplicationResponse>> GetPhotographerApplicationsAsync(int photographerId);
        Task<bool> WithdrawApplicationAsync(int eventId, int photographerId);
        Task<IEnumerable<EventPhotographerResponse>> GetApprovedPhotographersAsync(int eventId);

        // Event Bookings
        Task<EventBookingResponse> CreateEventBookingAsync(EventBookingRequest request);
        Task<EventBookingResponse> GetEventBookingByIdAsync(int eventBookingId);
        Task<IEnumerable<EventBookingResponse>> GetEventBookingsAsync(int eventId);
        Task<IEnumerable<EventBookingResponse>> GetUserEventBookingsAsync(int userId);
        Task<bool> CancelEventBookingAsync(int eventBookingId);

        // Event Statistics
        Task<EventStatisticsResponse> GetEventStatisticsAsync(int eventId);
        Task<IEnumerable<EventListResponse>> GetEventListAsync();
        Task<IEnumerable<EventListResponse>> GetEventsByStatusAsync(string status);
        Task<IEnumerable<EventListResponse>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Event Discovery
        Task<IEnumerable<EventListResponse>> SearchEventsAsync(string searchTerm);
        Task<IEnumerable<EventListResponse>> GetFeaturedEventsAsync();
        Task<IEnumerable<EventListResponse>> GetEventsNearbyAsync(double latitude, double longitude, double radiusKm);
    }
}
