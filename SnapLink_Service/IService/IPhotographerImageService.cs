using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService
{
    public interface IPhotographerImageService
    {
        // Get photographer image by ID
        Task<PhotographerImageResponse> GetByIdAsync(int id);
        
        // Get all images for a photographer
        Task<IEnumerable<PhotographerImageResponse>> GetByPhotographerAsync(int photographerId);
        
        // Get primary image for a photographer
        Task<PhotographerImageResponse?> GetPrimaryImageAsync(int photographerId);
        
        // Create new photographer image
        Task<PhotographerImageResponse> CreateAsync(CreatePhotographerImageRequest request);
        
        // Update photographer image
        Task<PhotographerImageResponse> UpdateAsync(UpdatePhotographerImageRequest request);
        
        // Delete photographer image
        Task<bool> DeleteAsync(int id);
        
        // Set image as primary (and unset others)
        Task<bool> SetAsPrimaryAsync(int imageId);
        
        // Get all images with photographer details
        Task<IEnumerable<PhotographerImageResponse>> GetAllWithPhotographerAsync();
    }
} 