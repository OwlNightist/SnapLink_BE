using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService
{
    public interface IReviewService
    {
        // Get all reviews
        Task<IEnumerable<ReviewResponse>> GetAllReviewsAsync();
        
        // Get review by ID
        Task<ReviewResponse> GetReviewByIdAsync(int id);
        
        // Get reviews by photographer
        Task<IEnumerable<ReviewResponse>> GetReviewsByPhotographerAsync(int photographerId);
        
        // Get reviews by booking
        Task<IEnumerable<ReviewResponse>> GetReviewsByBookingAsync(int bookingId);
        
        // Create new review
        Task<ReviewResponse> CreateReviewAsync(CreateReviewRequest request);
        
        // Update review
        Task<ReviewResponse> UpdateReviewAsync(int id, UpdateReviewRequest request);
        
        // Delete review
        Task<bool> DeleteReviewAsync(int id);
        
        // Get average rating for photographer
        Task<decimal?> GetAverageRatingForPhotographerAsync(int photographerId);
    }
} 