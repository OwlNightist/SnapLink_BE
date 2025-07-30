using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService;

public interface IBookingService
{
    Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request, int userId);
    Task<BookingResponse> GetBookingByIdAsync(int bookingId);
    Task<BookingListResponse> GetUserBookingsAsync(int userId, int page = 1, int pageSize = 10);
    Task<BookingListResponse> GetPhotographerBookingsAsync(int photographerId, int page = 1, int pageSize = 10);
    Task<BookingResponse> UpdateBookingAsync(int bookingId, UpdateBookingRequest request);
    Task<BookingResponse> CancelBookingAsync(int bookingId);
    Task<BookingResponse> CompleteBookingAsync(int bookingId);
    Task<bool> IsPhotographerAvailableAsync(int photographerId, DateTime startTime, DateTime endTime);
    Task<bool> IsLocationAvailableAsync(int locationId, DateTime startTime, DateTime endTime);
    Task<bool> IsLocationAvailableForPhotographerAsync(int locationId, int photographerId, DateTime startTime, DateTime endTime);
    Task<IEnumerable<BookingData>> GetPhotographersAtLocationAsync(int locationId, DateTime startTime, DateTime endTime);
    Task<decimal> CalculateBookingPriceAsync(int photographerId, int? locationId, DateTime startTime, DateTime endTime);
    Task<int> CancelExpiredPendingBookingsAsync();
    Task<int> CancelAllPendingBookingsAsync();
} 