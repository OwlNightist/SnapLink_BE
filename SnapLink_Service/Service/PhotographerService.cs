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
    public class PhotographerService : IPhotographerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PhotographerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PhotographerListResponse>> GetAllPhotographersAsync()
        {
            var photographers = await _unitOfWork.PhotographerRepository.GetAsync(
                includeProperties: "User,PhotographerStyles.Style"
            );
            return _mapper.Map<IEnumerable<PhotographerListResponse>>(photographers);
        }

        public async Task<PhotographerResponse> GetPhotographerByIdAsync(int id)
        {
            var photographer = await _unitOfWork.PhotographerRepository.GetAsync(
                filter: p => p.PhotographerId == id,
                includeProperties: "User,PhotographerStyles.Style"
            );
            
            var photographerEntity = photographer.FirstOrDefault();
            if (photographerEntity == null)
                throw new ArgumentException($"Photographer with ID {id} not found");
                
            return _mapper.Map<PhotographerResponse>(photographerEntity);
        }

        public async Task<PhotographerDetailResponse> GetPhotographerDetailAsync(int id)
        {
            var photographer = await _unitOfWork.PhotographerRepository.GetAsync(
                filter: p => p.PhotographerId == id,
                includeProperties: "User,Bookings,PhotographerStyles.Style"
            );
            
            var photographerEntity = photographer.FirstOrDefault();
            if (photographerEntity == null)
                throw new ArgumentException($"Photographer with ID {id} not found");
                
            return _mapper.Map<PhotographerDetailResponse>(photographerEntity);
        }


        public async Task<PhotographerResponse> CreatePhotographerAsync(CreatePhotographerRequest request)
        {
            // Check if user exists
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
            if (user == null)
                throw new ArgumentException($"User with ID {request.UserId} not found");

            // Check if photographer already exists for this user
            var existingPhotographer = await _unitOfWork.PhotographerRepository.GetAsync(
                filter: p => p.UserId == request.UserId
            );
            if (existingPhotographer.Any())
                throw new InvalidOperationException($"Photographer already exists for user {request.UserId}");

            var photographer = _mapper.Map<Photographer>(request);
            await _unitOfWork.PhotographerRepository.AddAsync(photographer);
            await _unitOfWork.SaveChangesAsync();

            // Add styles if provided
            if (request.StyleIds != null && request.StyleIds.Any())
            {
                foreach (var styleId in request.StyleIds)
                {
                    var style = await _unitOfWork.StyleRepository.GetByIdAsync(styleId);
                    if (style != null)
                    {
                        var photographerStyle = new PhotographerStyle
                        {
                            PhotographerId = photographer.PhotographerId,
                            StyleId = styleId
                        };
                        await _unitOfWork.PhotographerStyleRepository.AddAsync(photographerStyle);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
            }

            // Get the created photographer with user details and styles
            var createdPhotographer = await _unitOfWork.PhotographerRepository.GetAsync(
                filter: p => p.PhotographerId == photographer.PhotographerId,
                includeProperties: "User,PhotographerStyles.Style"
            );

            return _mapper.Map<PhotographerResponse>(createdPhotographer.FirstOrDefault());
        }

        public async Task<PhotographerResponse> UpdatePhotographerAsync(int id, UpdatePhotographerRequest request)
        {
            var photographer = await _unitOfWork.PhotographerRepository.GetByIdAsync(id);
            if (photographer == null)
                throw new ArgumentException($"Photographer with ID {id} not found");

            _mapper.Map(request, photographer);
            _unitOfWork.PhotographerRepository.Update(photographer);

            // Update styles if provided
            if (request.StyleIds != null)
            {
                // Remove existing styles
                var existingStyles = await _unitOfWork.PhotographerStyleRepository.GetAsync(
                    filter: ps => ps.PhotographerId == id
                );
                foreach (var style in existingStyles)
                {
                    _unitOfWork.PhotographerStyleRepository.Remove(style);
                }

                // Add new styles
                foreach (var styleId in request.StyleIds)
                {
                    var style = await _unitOfWork.StyleRepository.GetByIdAsync(styleId);
                    if (style != null)
                    {
                        var photographerStyle = new PhotographerStyle
                        {
                            PhotographerId = id,
                            StyleId = styleId
                        };
                        await _unitOfWork.PhotographerStyleRepository.AddAsync(photographerStyle);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            // Get the updated photographer with user details and styles
            var updatedPhotographer = await _unitOfWork.PhotographerRepository.GetAsync(
                filter: p => p.PhotographerId == id,
                includeProperties: "User,PhotographerStyles.Style"
            );

            return _mapper.Map<PhotographerResponse>(updatedPhotographer.FirstOrDefault());
        }

        public async Task<bool> DeletePhotographerAsync(int id)
        {
            var photographer = await _unitOfWork.PhotographerRepository.GetByIdAsync(id);
            if (photographer == null)
                return false;

            // Remove associated styles first
            var photographerStyles = await _unitOfWork.PhotographerStyleRepository.GetAsync(
                filter: ps => ps.PhotographerId == id
            );
            foreach (var style in photographerStyles)
            {
                _unitOfWork.PhotographerStyleRepository.Remove(style);
            }

            _unitOfWork.PhotographerRepository.Remove(photographer);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAvailabilityAsync(int id, string availabilityStatus)
        {
            var photographer = await _unitOfWork.PhotographerRepository.GetByIdAsync(id);
            if (photographer == null)
                return false;

            photographer.AvailabilityStatus = availabilityStatus;
            _unitOfWork.PhotographerRepository.Update(photographer);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateRatingAsync(int id, decimal rating)
        {
            var photographer = await _unitOfWork.PhotographerRepository.GetByIdAsync(id);
            if (photographer == null)
                return false;

            photographer.Rating = rating;
            _unitOfWork.PhotographerRepository.Update(photographer);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateRatingFromReviewAsync(int photographerId, decimal newRating)
        {
            var photographer = await _unitOfWork.PhotographerRepository.GetByIdAsync(photographerId);
            if (photographer == null)
                return false;

            // Initialize rating fields if they're null
            if (photographer.RatingSum == null)
                photographer.RatingSum = 0;
            if (photographer.RatingCount == null)
                photographer.RatingCount = 0;

            // Add the new rating
            photographer.RatingSum += newRating;
            photographer.RatingCount += 1;

            // Calculate the new average rating
            photographer.Rating = photographer.RatingSum / photographer.RatingCount;

            _unitOfWork.PhotographerRepository.Update(photographer);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> VerifyPhotographerAsync(int id, string verificationStatus)
        {
            var photographer = await _unitOfWork.PhotographerRepository.GetByIdAsync(id);
            if (photographer == null)
                return false;

            photographer.VerificationStatus = verificationStatus;
            _unitOfWork.PhotographerRepository.Update(photographer);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddStyleToPhotographerAsync(int photographerId, int styleId)
        {
            // Check if photographer exists
            var photographer = await _unitOfWork.PhotographerRepository.GetByIdAsync(photographerId);
            if (photographer == null)
                return false;

            // Check if style exists
            var style = await _unitOfWork.StyleRepository.GetByIdAsync(styleId);
            if (style == null)
                return false;

            // Check if relationship already exists
            var existingStyle = await _unitOfWork.PhotographerStyleRepository.GetAsync(
                filter: ps => ps.PhotographerId == photographerId && ps.StyleId == styleId
            );
            if (existingStyle.Any())
                return false; // Already exists

            var photographerStyle = new PhotographerStyle
            {
                PhotographerId = photographerId,
                StyleId = styleId
            };

            await _unitOfWork.PhotographerStyleRepository.AddAsync(photographerStyle);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveStyleFromPhotographerAsync(int photographerId, int styleId)
        {
            var photographerStyle = await _unitOfWork.PhotographerStyleRepository.GetAsync(
                filter: ps => ps.PhotographerId == photographerId && ps.StyleId == styleId
            );

            var styleToRemove = photographerStyle.FirstOrDefault();
            if (styleToRemove == null)
                return false;

            _unitOfWork.PhotographerStyleRepository.Remove(styleToRemove);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<string>> GetPhotographerStylesAsync(int photographerId)
        {
            var photographerStyles = await _unitOfWork.PhotographerStyleRepository.GetAsync(
                filter: ps => ps.PhotographerId == photographerId,
                includeProperties: "Style"
            );

            return photographerStyles.Select(ps => ps.Style.Name);
        }
    }
}
