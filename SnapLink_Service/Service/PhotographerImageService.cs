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
    public class PhotographerImageService : IPhotographerImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PhotographerImageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PhotographerImageResponse> GetByIdAsync(int id)
        {
            var image = await _unitOfWork.PhotographerImageRepository.GetAsync(
                filter: pi => pi.PhotographerImageId == id,
                includeProperties: "Photographer"
            );
            
            var imageEntity = image.FirstOrDefault();
            if (imageEntity == null)
                throw new ArgumentException($"Photographer image with ID {id} not found");
                
            return _mapper.Map<PhotographerImageResponse>(imageEntity);
        }

        public async Task<IEnumerable<PhotographerImageResponse>> GetByPhotographerAsync(int photographerId)
        {
            // Verify photographer exists
            var photographer = await _unitOfWork.PhotographerRepository.GetByIdAsync(photographerId);
            if (photographer == null)
                throw new ArgumentException($"Photographer with ID {photographerId} not found");

            var images = await _unitOfWork.PhotographerImageRepository.GetAsync(
                filter: pi => pi.PhotographerId == photographerId,
                orderBy: q => q.OrderByDescending(pi => pi.IsPrimary).ThenBy(pi => pi.UploadedAt),
                includeProperties: "Photographer"
            );
            
            return _mapper.Map<IEnumerable<PhotographerImageResponse>>(images);
        }

        public async Task<PhotographerImageResponse?> GetPrimaryImageAsync(int photographerId)
        {
            // Verify photographer exists
            var photographer = await _unitOfWork.PhotographerRepository.GetByIdAsync(photographerId);
            if (photographer == null)
                throw new ArgumentException($"Photographer with ID {photographerId} not found");

            var primaryImage = await _unitOfWork.PhotographerImageRepository.GetAsync(
                filter: pi => pi.PhotographerId == photographerId && pi.IsPrimary == true,
                includeProperties: "Photographer"
            );
            
            var imageEntity = primaryImage.FirstOrDefault();
            return imageEntity != null ? _mapper.Map<PhotographerImageResponse>(imageEntity) : null;
        }

        public async Task<PhotographerImageResponse> CreateAsync(CreatePhotographerImageRequest request)
        {
            // Verify photographer exists
            var photographer = await _unitOfWork.PhotographerRepository.GetByIdAsync(request.PhotographerId);
            if (photographer == null)
                throw new ArgumentException($"Photographer with ID {request.PhotographerId} not found");

            var image = _mapper.Map<PhotographerImage>(request);
            image.UploadedAt = DateTime.UtcNow;

            // If this is set as primary, unset other primary images
            if (request.IsPrimary)
            {
                var existingPrimaryImages = await _unitOfWork.PhotographerImageRepository.GetAsync(
                    filter: pi => pi.PhotographerId == request.PhotographerId && pi.IsPrimary == true
                );
                
                foreach (var existingImage in existingPrimaryImages)
                {
                    existingImage.IsPrimary = false;
                    _unitOfWork.PhotographerImageRepository.Update(existingImage);
                }
            }

            await _unitOfWork.PhotographerImageRepository.AddAsync(image);
            await _unitOfWork.SaveChangesAsync();

            // Get the created image with photographer details
            var createdImage = await _unitOfWork.PhotographerImageRepository.GetAsync(
                filter: pi => pi.PhotographerImageId == image.PhotographerImageId,
                includeProperties: "Photographer"
            );

            return _mapper.Map<PhotographerImageResponse>(createdImage.FirstOrDefault());
        }

        public async Task<PhotographerImageResponse> UpdateAsync(UpdatePhotographerImageRequest request)
        {
            var image = await _unitOfWork.PhotographerImageRepository.GetByIdAsync(request.PhotographerImageId);
            if (image == null)
                throw new ArgumentException($"Photographer image with ID {request.PhotographerImageId} not found");

            // Update only provided fields
            if (request.ImageUrl != null)
                image.ImageUrl = request.ImageUrl;
            if (request.Caption != null)
                image.Caption = request.Caption;
            if (request.IsPrimary.HasValue)
            {
                image.IsPrimary = request.IsPrimary.Value;
                
                // If setting as primary, unset other primary images
                if (request.IsPrimary.Value)
                {
                    var existingPrimaryImages = await _unitOfWork.PhotographerImageRepository.GetAsync(
                        filter: pi => pi.PhotographerId == image.PhotographerId && 
                                    pi.PhotographerImageId != request.PhotographerImageId && 
                                    pi.IsPrimary == true
                    );
                    
                    foreach (var existingImage in existingPrimaryImages)
                    {
                        existingImage.IsPrimary = false;
                        _unitOfWork.PhotographerImageRepository.Update(existingImage);
                    }
                }
            }

            _unitOfWork.PhotographerImageRepository.Update(image);
            await _unitOfWork.SaveChangesAsync();

            // Get the updated image with photographer details
            var updatedImage = await _unitOfWork.PhotographerImageRepository.GetAsync(
                filter: pi => pi.PhotographerImageId == request.PhotographerImageId,
                includeProperties: "Photographer"
            );

            return _mapper.Map<PhotographerImageResponse>(updatedImage.FirstOrDefault());
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var image = await _unitOfWork.PhotographerImageRepository.GetByIdAsync(id);
            if (image == null)
                return false;

            _unitOfWork.PhotographerImageRepository.Remove(image);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetAsPrimaryAsync(int imageId)
        {
            var image = await _unitOfWork.PhotographerImageRepository.GetByIdAsync(imageId);
            if (image == null)
                return false;

            // Unset all other primary images for this photographer
            var existingPrimaryImages = await _unitOfWork.PhotographerImageRepository.GetAsync(
                filter: pi => pi.PhotographerId == image.PhotographerId && pi.IsPrimary == true
            );
            
            foreach (var existingImage in existingPrimaryImages)
            {
                existingImage.IsPrimary = false;
                _unitOfWork.PhotographerImageRepository.Update(existingImage);
            }

            // Set this image as primary
            image.IsPrimary = true;
            _unitOfWork.PhotographerImageRepository.Update(image);
            await _unitOfWork.SaveChangesAsync();
            
            return true;
        }

        public async Task<IEnumerable<PhotographerImageResponse>> GetAllWithPhotographerAsync()
        {
            var images = await _unitOfWork.PhotographerImageRepository.GetAsync(
                orderBy: q => q.OrderBy(pi => pi.PhotographerId).ThenByDescending(pi => pi.IsPrimary),
                includeProperties: "Photographer"
            );
            
            return _mapper.Map<IEnumerable<PhotographerImageResponse>>(images);
        }
    }
} 