using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService
{
    public interface IPhotographerService
    {
        // Get all photographers
        Task<IEnumerable<PhotographerListResponse>> GetAllPhotographersAsync();
        
        // Get photographer by ID
        Task<PhotographerResponse> GetPhotographerByIdAsync(int id);
        
        // Get photographer with details
        Task<PhotographerDetailResponse> GetPhotographerDetailAsync(int id);
        
        // Get photographers by specialty
        Task<IEnumerable<PhotographerListResponse>> GetPhotographersBySpecialtyAsync(string specialty);
        
        // Get available photographers
        Task<IEnumerable<PhotographerListResponse>> GetAvailablePhotographersAsync();
        
        // Get featured photographers
        Task<IEnumerable<PhotographerListResponse>> GetFeaturedPhotographersAsync();
        
        // Create new photographer
        Task<PhotographerResponse> CreatePhotographerAsync(CreatePhotographerRequest request);
        
        // Update photographer
        Task<PhotographerResponse> UpdatePhotographerAsync(int id, UpdatePhotographerRequest request);
        
        // Delete photographer
        Task<bool> DeletePhotographerAsync(int id);
        
        // Update photographer availability
        Task<bool> UpdateAvailabilityAsync(int id, string availabilityStatus);
        
        // Update photographer rating
        Task<bool> UpdateRatingAsync(int id, decimal rating);
        
        // Update photographer rating from review
        Task<bool> UpdateRatingFromReviewAsync(int photographerId, decimal newRating);
        
        // Verify photographer
        Task<bool> VerifyPhotographerAsync(int id, string verificationStatus);
    }
}
