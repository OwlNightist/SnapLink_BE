using SnapLink_Repository.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SnapLink_Repository.IRepository
{
    public interface IPhotoDeliveryRepository
    {
        Task<PhotoDelivery?> GetPhotoDeliveryByBookingIdAsync(int bookingId);
        Task<PhotoDelivery?> GetPhotoDeliveryByIdAsync(int photoDeliveryId);
        Task<IEnumerable<PhotoDelivery>> GetPhotoDeliveriesByPhotographerIdAsync(int photographerId);
        Task<IEnumerable<PhotoDelivery>> GetPhotoDeliveriesByCustomerIdAsync(int customerId);
        Task<IEnumerable<PhotoDelivery>> GetPhotoDeliveriesByStatusAsync(string status);
        Task<IEnumerable<PhotoDelivery>> GetPendingPhotoDeliveriesAsync();
        Task<IEnumerable<PhotoDelivery>> GetExpiredPhotoDeliveriesAsync();
        Task AddPhotoDeliveryAsync(PhotoDelivery photoDelivery);
        Task UpdatePhotoDeliveryAsync(PhotoDelivery photoDelivery);
        Task DeletePhotoDeliveryAsync(PhotoDelivery photoDelivery);
        Task SaveChangesAsync();
    }
} 