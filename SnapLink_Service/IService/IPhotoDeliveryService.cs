using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SnapLink_Service.IService
{
    public interface IPhotoDeliveryService
    {
        // Core operations
        Task<PhotoDeliveryResponse> CreatePhotoDeliveryAsync(CreatePhotoDeliveryRequest request);
        Task<PhotoDeliveryResponse> GetPhotoDeliveryByBookingIdAsync(int bookingId);
        Task<PhotoDeliveryResponse> GetPhotoDeliveryByIdAsync(int photoDeliveryId);
        Task<PhotoDeliveryResponse> UpdatePhotoDeliveryAsync(int photoDeliveryId, UpdatePhotoDeliveryRequest request);
        Task<bool> DeletePhotoDeliveryAsync(int photoDeliveryId);

        // Photographer operations
        Task<PhotoDeliveryResponse> UploadPhotosAsync(UploadPhotosRequest request);
        Task<IEnumerable<PhotoDeliveryResponse>> GetPhotoDeliveriesByPhotographerIdAsync(int photographerId);
        Task<IEnumerable<PhotoDeliveryResponse>> GetPendingPhotoDeliveriesForPhotographerAsync(int photographerId);

        // Customer operations
        Task<IEnumerable<PhotoDeliveryResponse>> GetPhotoDeliveriesByCustomerIdAsync(int customerId);
        Task<PhotoDeliveryResponse> MarkAsDeliveredAsync(MarkAsDeliveredRequest request);

        // Admin operations
        Task<IEnumerable<PhotoDeliveryResponse>> GetPhotoDeliveriesByStatusAsync(string status);
        Task<IEnumerable<PhotoDeliveryResponse>> GetPendingPhotoDeliveriesAsync();
        Task<IEnumerable<PhotoDeliveryResponse>> GetExpiredPhotoDeliveriesAsync();

        // Business logic
        Task<bool> IsPhotoDeliveryRequiredAsync(int bookingId);
        Task<string> GenerateDriveLinkAsync(int bookingId);
        Task<bool> ValidateDriveLinkAsync(string driveLink);
        Task SendPhotoDeliveryNotificationAsync(int bookingId, string driveLink);
    }
} 