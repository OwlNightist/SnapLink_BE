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
    public class ImageService : IImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAzureStorageService _azureStorageService;

        public ImageService(IUnitOfWork unitOfWork, IMapper mapper, IAzureStorageService azureStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _azureStorageService = azureStorageService;
        }

        public async Task<ImageResponse> GetByIdAsync(int id)
        {
            var image = await _unitOfWork.ImageRepository.GetAsync(
                filter: img => img.Id == id
            );
            
            var imageEntity = image.FirstOrDefault();
            if (imageEntity == null)
                throw new ArgumentException($"Image with ID {id} not found");
                
            return _mapper.Map<ImageResponse>(imageEntity);
        }

        public async Task<IEnumerable<ImageResponse>> GetByUserIdAsync(int userId)
        {
            var images = await _unitOfWork.ImageRepository.GetAsync(
                filter: img => img.UserId == userId,
                orderBy: q => q.OrderByDescending(img => img.IsPrimary).ThenBy(img => img.CreatedAt)
            );
            return _mapper.Map<IEnumerable<ImageResponse>>(images);
        }

        public async Task<IEnumerable<ImageResponse>> GetByPhotographerIdAsync(int photographerId)
        {
            var images = await _unitOfWork.ImageRepository.GetAsync(
                filter: img => img.PhotographerId == photographerId,
                orderBy: q => q.OrderByDescending(img => img.IsPrimary).ThenBy(img => img.CreatedAt)
            );
            return _mapper.Map<IEnumerable<ImageResponse>>(images);
        }

        public async Task<IEnumerable<ImageResponse>> GetByLocationIdAsync(int locationId)
        {
            var images = await _unitOfWork.ImageRepository.GetAsync(
                filter: img => img.LocationId == locationId,
                orderBy: q => q.OrderByDescending(img => img.IsPrimary).ThenBy(img => img.CreatedAt)
            );
            return _mapper.Map<IEnumerable<ImageResponse>>(images);
        }

        public async Task<ImageResponse?> GetPrimaryByUserIdAsync(int userId)
        {
            var primaryImage = await _unitOfWork.ImageRepository.GetAsync(
                filter: img => img.UserId == userId && img.IsPrimary
            );
            var imageEntity = primaryImage.FirstOrDefault();
            return imageEntity != null ? _mapper.Map<ImageResponse>(imageEntity) : null;
        }

        public async Task<ImageResponse?> GetPrimaryByPhotographerIdAsync(int photographerId)
        {
            var primaryImage = await _unitOfWork.ImageRepository.GetAsync(
                filter: img => img.PhotographerId == photographerId && img.IsPrimary
            );
            var imageEntity = primaryImage.FirstOrDefault();
            return imageEntity != null ? _mapper.Map<ImageResponse>(imageEntity) : null;
        }

        public async Task<ImageResponse?> GetPrimaryByLocationIdAsync(int locationId)
        {
            var primaryImage = await _unitOfWork.ImageRepository.GetAsync(
                filter: img => img.LocationId == locationId && img.IsPrimary
            );
            var imageEntity = primaryImage.FirstOrDefault();
            return imageEntity != null ? _mapper.Map<ImageResponse>(imageEntity) : null;
        }


        public async Task<ImageResponse> UploadImageAsync(UploadImageRequest request)
        {
            // Upload file to Azure Storage
            var blobName = await _azureStorageService.UploadImageAsync(
                request.File,
                request.UserId,
                request.PhotographerId,
                request.LocationId,
                request.PhotographerEventId
            );
            // Get the public URL
            var imageUrl = await _azureStorageService.GetImageUrlAsync(blobName);

            // Create image entity
            var image = new Image
            {
                Url = imageUrl,
                UserId = request.UserId,
                PhotographerId = request.PhotographerId,
                LocationId = request.LocationId,
                IsPrimary = request.IsPrimary,
                Caption = request.Caption,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.ImageRepository.AddAsync(image);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ImageResponse>(image);
        }

        public async Task<ImageResponse> UpdateAsync(UpdateImageRequest request)
        {
            var image = await _unitOfWork.ImageRepository.GetAsync(
                filter: img => img.Id == request.Id
            );
            var imageEntity = image.FirstOrDefault();
            if (imageEntity == null)
                throw new ArgumentException($"Image with ID {request.Id} not found");

            // Update only provided fields
            if (request.UserId.HasValue)
                imageEntity.UserId = request.UserId.Value;
            if (!string.IsNullOrEmpty(request.Url))
                imageEntity.Url = request.Url;
            if (request.Caption != null)
                imageEntity.Caption = request.Caption;
            if (request.IsPrimary.HasValue)
            {
                imageEntity.IsPrimary = request.IsPrimary.Value;

                // If setting as primary, unset other primary images for the same entity
                if (request.IsPrimary.Value)
                {
                    var existingPrimaryImages = await _unitOfWork.ImageRepository.GetAsync(
                        filter: img =>
                            ((img.UserId != null && img.UserId == imageEntity.UserId) ||
                             (img.PhotographerId != null && img.PhotographerId == imageEntity.PhotographerId) ||
                             (img.LocationId != null && img.LocationId == imageEntity.LocationId)
                            ) &&
                            img.Id != request.Id &&
                            img.IsPrimary
                    );
                    foreach (var existingImage in existingPrimaryImages)
                    {
                        existingImage.IsPrimary = false;
                        _unitOfWork.ImageRepository.Update(existingImage);
                    }
                }
            }

            _unitOfWork.ImageRepository.Update(imageEntity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ImageResponse>(imageEntity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var image = await _unitOfWork.ImageRepository.GetAsync(
                filter: img => img.Id == id
            );
            
            var imageEntity = image.FirstOrDefault();
            if (imageEntity == null)
                return false;

            // Extract blob name from URL and delete from Azure Storage
            if (!string.IsNullOrEmpty(imageEntity.Url))
            {
                try
                {
                    var uri = new Uri(imageEntity.Url);
                    var blobName = uri.AbsolutePath.TrimStart('/').Replace(_azureStorageService.GetContainerName(), "").TrimStart('/');
                    await _azureStorageService.DeleteImageAsync(blobName);
                }
                catch (Exception ex)
                {
                    // Log the error but don't fail the deletion
                    Console.WriteLine($"Error deleting blob: {ex.Message}");
                }
            }

            _unitOfWork.ImageRepository.Remove(imageEntity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetAsPrimaryAsync(int imageId)
        {
            var image = await _unitOfWork.ImageRepository.GetAsync(
                filter: img => img.Id == imageId
            );
            var imageEntity = image.FirstOrDefault();
            if (imageEntity == null)
                return false;

            // Unset all other primary images for the same entity
            var existingPrimaryImages = await _unitOfWork.ImageRepository.GetAsync(
                filter: img =>
                    ((img.UserId != null && img.UserId == imageEntity.UserId) ||
                     (img.PhotographerId != null && img.PhotographerId == imageEntity.PhotographerId) ||
                     (img.LocationId != null && img.LocationId == imageEntity.LocationId) 
                     ) &&
                    img.Id != imageId &&
                    img.IsPrimary
            );
            foreach (var existingImage in existingPrimaryImages)
            {
                existingImage.IsPrimary = false;
                _unitOfWork.ImageRepository.Update(existingImage);
            }

            // Set this image as primary
            imageEntity.IsPrimary = true;
            _unitOfWork.ImageRepository.Update(imageEntity);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
} 