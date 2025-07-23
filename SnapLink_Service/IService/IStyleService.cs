using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService
{
    public interface IStyleService
    {
        // Get all styles
        Task<IEnumerable<StyleResponse>> GetAllStylesAsync();
        
        // Get style by ID
        Task<StyleResponse> GetStyleByIdAsync(int id);
        
        // Get style with detailed information including photographers
        Task<StyleDetailResponse> GetStyleDetailAsync(int id);
        
        // Get styles by name (search)
        Task<IEnumerable<StyleResponse>> GetStylesByNameAsync(string name);
        
        // Get popular styles (with most photographers)
        Task<IEnumerable<StyleResponse>> GetPopularStylesAsync(int count = 10);
        
        // Create new style
        Task<StyleResponse> CreateStyleAsync(CreateStyleRequest request);
        
        // Update style
        Task<StyleResponse> UpdateStyleAsync(int id, UpdateStyleRequest request);
        
        // Delete style
        Task<bool> DeleteStyleAsync(int id);
        
        // Get styles with photographer count
        Task<IEnumerable<StyleResponse>> GetStylesWithPhotographerCountAsync();
    }
} 