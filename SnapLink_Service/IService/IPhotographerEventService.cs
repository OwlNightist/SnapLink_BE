using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService
{
    public interface IPhotographerEventService
    {
        Task<IEnumerable<PhotographerEventListResponse>> GetAllEventsAsync();
        Task<PhotographerEventResponse> GetEventByIdAsync(int eventId);
        Task<PhotographerEventResponse> CreateEventAsync(int photographerId, CreatePhotographerEventRequest request);
        Task<PhotographerEventResponse> UpdateEventAsync(int eventId, UpdatePhotographerEventRequest request);
        Task<bool> DeleteEventAsync(int eventId);
    }
} 