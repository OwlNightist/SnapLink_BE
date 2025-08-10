using SnapLink_Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Repository.IRepository
{
    public interface IRatingRepository
    {
        Task<Rating?> GetByIdAsync(int id);
        Task<IEnumerable<Rating>> GetAllAsync();
        Task<IEnumerable<Rating>> GetByPhotographerAsync(int photographerId);
        Task<IEnumerable<Rating>> GetByLocationAsync(int locationId);
        Task<Rating?> GetExistingAsync(int bookingId, int reviewerUserId, int? photographerId, int? locationId);

        Task AddAsync(Rating rating);
        Task UpdateAsync(Rating rating);
        Task DeleteAsync(Rating rating);
        Task SaveChangesAsync();

        // hỗ trợ validate/aggregate
        Task<Booking?> GetBookingAsync(int bookingId);
        Task<Photographer?> GetPhotographerAsync(int photographerId);
        Task<Location?> GetLocationAsync(int locationId);
    }
}
