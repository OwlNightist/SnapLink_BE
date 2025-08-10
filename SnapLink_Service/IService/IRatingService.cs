using SnapLink_Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Service.IService
{
    public interface IRatingService
    {
        Task<RatingDto?> GetByIdAsync(int id);
        Task<IEnumerable<RatingDto>> GetAllAsync();
        Task<IEnumerable<RatingDto>> GetByPhotographerAsync(int photographerId);
        Task<IEnumerable<RatingDto>> GetByLocationAsync(int locationId);

        Task<int> CreateAsync(CreateRatingDto dto);
        Task UpdateAsync(int ratingId, UpdateRatingDto dto);
        Task DeleteAsync(int ratingId);
    }
}
