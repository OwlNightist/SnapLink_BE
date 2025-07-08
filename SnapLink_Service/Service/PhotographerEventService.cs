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
    public class PhotographerEventService : IPhotographerEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PhotographerEventService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PhotographerEventListResponse>> GetAllEventsAsync()
        {
            var events = await _unitOfWork.PhotographerEventRepository.GetAsync(
                includeProperties: "Photographer,PhotographerEventLocations.Location"
            );
            return _mapper.Map<IEnumerable<PhotographerEventListResponse>>(events);
        }

        public async Task<PhotographerEventResponse> GetEventByIdAsync(int eventId)
        {
            var events = await _unitOfWork.PhotographerEventRepository.GetAsync(
                filter: e => e.EventId == eventId,
                includeProperties: "Photographer,PhotographerEventLocations.Location"
            );
            var eventEntity = events.FirstOrDefault();
            if (eventEntity == null)
                throw new ArgumentException($"Event with ID {eventId} not found");
            return _mapper.Map<PhotographerEventResponse>(eventEntity);
        }

        public async Task<PhotographerEventResponse> CreateEventAsync(int photographerId, CreatePhotographerEventRequest request)
        {
            ValidateEventRequest(request.StartDate, request.EndDate, request.DiscountPercentage);

            var photographer = await _unitOfWork.PhotographerRepository.GetByIdAsync(photographerId);
            if (photographer == null)
                throw new ArgumentException($"Photographer with ID {photographerId} not found");

            var eventEntity = _mapper.Map<PhotographerEvent>(request);
            eventEntity.PhotographerId = photographerId;
            eventEntity.Status = "Active";
            eventEntity.CreatedAt = DateTime.UtcNow;
            eventEntity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.PhotographerEventRepository.AddAsync(eventEntity);
            await _unitOfWork.SaveChangesAsync();

            // Add locations
            if (request.LocationIds != null && request.LocationIds.Any())
            {
                foreach (var locId in request.LocationIds)
                {
                    var location = await _unitOfWork.LocationRepository.GetByIdAsync(locId);
                    if (location != null)
                    {
                        var eventLocation = new PhotographerEventLocation
                        {
                            EventId = eventEntity.EventId,
                            LocationId = locId
                        };
                        await _unitOfWork.PhotographerEventLocationRepository.AddAsync(eventLocation);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
            }

            var createdEvent = await _unitOfWork.PhotographerEventRepository.GetAsync(
                filter: e => e.EventId == eventEntity.EventId,
                includeProperties: "Photographer,PhotographerEventLocations.Location"
            );
            return _mapper.Map<PhotographerEventResponse>(createdEvent.FirstOrDefault());
        }

        public async Task<PhotographerEventResponse> UpdateEventAsync(int eventId, UpdatePhotographerEventRequest request)
        {
            ValidateEventRequest(request.StartDate, request.EndDate, request.DiscountPercentage);

            var eventEntity = await _unitOfWork.PhotographerEventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
                throw new ArgumentException($"Event with ID {eventId} not found");

            _mapper.Map(request, eventEntity);
            eventEntity.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.PhotographerEventRepository.Update(eventEntity);

            // Update locations
            if (request.LocationIds != null)
            {
                var existingLocations = await _unitOfWork.PhotographerEventLocationRepository.GetAsync(
                    filter: el => el.EventId == eventId
                );
                foreach (var el in existingLocations)
                {
                    _unitOfWork.PhotographerEventLocationRepository.Remove(el);
                }
                foreach (var locId in request.LocationIds)
                {
                    var location = await _unitOfWork.LocationRepository.GetByIdAsync(locId);
                    if (location != null)
                    {
                        var eventLocation = new PhotographerEventLocation
                        {
                            EventId = eventId,
                            LocationId = locId
                        };
                        await _unitOfWork.PhotographerEventLocationRepository.AddAsync(eventLocation);
                    }
                }
            }
            await _unitOfWork.SaveChangesAsync();

            var updatedEvent = await _unitOfWork.PhotographerEventRepository.GetAsync(
                filter: e => e.EventId == eventId,
                includeProperties: "Photographer,PhotographerEventLocations.Location"
            );
            return _mapper.Map<PhotographerEventResponse>(updatedEvent.FirstOrDefault());
        }

        public async Task<bool> DeleteEventAsync(int eventId)
        {
            var eventEntity = await _unitOfWork.PhotographerEventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
                return false;

            // Remove related locations
            var eventLocations = await _unitOfWork.PhotographerEventLocationRepository.GetAsync(
                filter: el => el.EventId == eventId
            );
            foreach (var el in eventLocations)
            {
                _unitOfWork.PhotographerEventLocationRepository.Remove(el);
            }
            _unitOfWork.PhotographerEventRepository.Remove(eventEntity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private void ValidateEventRequest(DateTime? startDate, DateTime? endDate, decimal? discountPercentage)
        {
            if (startDate.HasValue && endDate.HasValue && startDate >= endDate)
                throw new ArgumentException("StartDate must be before EndDate");
            if (discountPercentage.HasValue && (discountPercentage < 0 || discountPercentage > 100))
                throw new ArgumentException("DiscountPercentage must be between 0 and 100");
        }
    }
} 