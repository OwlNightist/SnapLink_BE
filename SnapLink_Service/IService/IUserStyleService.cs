using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService
{
    public interface IUserStyleService
    {
        // Get user's favorite styles
        Task<UserFavoriteStylesResponse> GetUserFavoriteStylesAsync(int userId);
        
        // Add a style to user's favorites
        Task<UserStyleResponse> AddUserStyleAsync(AddUserStyleRequest request);
        
        // Update user's favorite styles (replace all)
        Task<UserFavoriteStylesResponse> UpdateUserStylesAsync(UpdateUserStylesRequest request);
        
        // Remove a style from user's favorites
        Task<bool> RemoveUserStyleAsync(int userId, int styleId);
        
        // Get style recommendations based on user's favorites
        Task<IEnumerable<StyleRecommendationResponse>> GetStyleRecommendationsAsync(int userId, int count = 10);
        
        // Get photographers recommended based on user's favorite styles
        Task<IEnumerable<RecommendedPhotographerInfo>> GetRecommendedPhotographersAsync(int userId, int count = 10);
        
        // Check if user has a specific style as favorite
        Task<bool> IsUserStyleFavoriteAsync(int userId, int styleId);
        
        // Get users who have a specific style as favorite
        Task<IEnumerable<int>> GetUsersByStyleAsync(int styleId);
    }
} 