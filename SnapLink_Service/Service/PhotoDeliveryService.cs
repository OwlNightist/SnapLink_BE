using AutoMapper;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Repository.Entity;
using SnapLink_Repository.IRepository;
using SnapLink_Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnapLink_Service.Service
{
    public class PhotoDeliveryService : IPhotoDeliveryService
    {
        private readonly IPhotoDeliveryRepository _photoDeliveryRepository;
        private readonly IMapper _mapper;

        public PhotoDeliveryService(IPhotoDeliveryRepository photoDeliveryRepository, IMapper mapper)
        {
            _photoDeliveryRepository = photoDeliveryRepository;
            _mapper = mapper;
        }

        public async Task<PhotoDeliveryResponse> CreatePhotoDeliveryAsync(CreatePhotoDeliveryRequest request)
        {
            // Check if photo delivery already exists for this booking
            var existingDelivery = await _photoDeliveryRepository.GetPhotoDeliveryByBookingIdAsync(request.BookingId);
            if (existingDelivery != null)
            {
                throw new InvalidOperationException($"Photo delivery already exists for booking {request.BookingId}");
            }

            var photoDelivery = _mapper.Map<PhotoDelivery>(request);
            
            // Set default status based on delivery method
            if (request.DeliveryMethod == "CustomerDevice")
            {
                photoDelivery.Status = "NotRequired";
            }
            else
            {
                photoDelivery.Status = "Pending";
            }

            await _photoDeliveryRepository.AddPhotoDeliveryAsync(photoDelivery);
            await _photoDeliveryRepository.SaveChangesAsync();

            var result = await _photoDeliveryRepository.GetPhotoDeliveryByIdAsync(photoDelivery.PhotoDeliveryId);
            return _mapper.Map<PhotoDeliveryResponse>(result);
        }

        public async Task<PhotoDeliveryResponse> GetPhotoDeliveryByBookingIdAsync(int bookingId)
        {
            var photoDelivery = await _photoDeliveryRepository.GetPhotoDeliveryByBookingIdAsync(bookingId);
            if (photoDelivery == null)
            {
                throw new InvalidOperationException($"Photo delivery not found for booking {bookingId}");
            }

            return _mapper.Map<PhotoDeliveryResponse>(photoDelivery);
        }

        public async Task<PhotoDeliveryResponse> GetPhotoDeliveryByIdAsync(int photoDeliveryId)
        {
            var photoDelivery = await _photoDeliveryRepository.GetPhotoDeliveryByIdAsync(photoDeliveryId);
            if (photoDelivery == null)
            {
                throw new InvalidOperationException($"Photo delivery not found with ID {photoDeliveryId}");
            }

            return _mapper.Map<PhotoDeliveryResponse>(photoDelivery);
        }

        public async Task<PhotoDeliveryResponse> UpdatePhotoDeliveryAsync(int photoDeliveryId, UpdatePhotoDeliveryRequest request)
        {
            var photoDelivery = await _photoDeliveryRepository.GetPhotoDeliveryByIdAsync(photoDeliveryId);
            if (photoDelivery == null)
            {
                throw new InvalidOperationException($"Photo delivery not found with ID {photoDeliveryId}");
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(request.DriveLink))
                photoDelivery.DriveLink = request.DriveLink;
            
            if (!string.IsNullOrEmpty(request.DriveFolderName))
                photoDelivery.DriveFolderName = request.DriveFolderName;
            
            if (request.PhotoCount.HasValue)
                photoDelivery.PhotoCount = request.PhotoCount;
            
            if (!string.IsNullOrEmpty(request.Status))
                photoDelivery.Status = request.Status;
            
            if (request.ExpiresAt.HasValue)
                photoDelivery.ExpiresAt = request.ExpiresAt;
            
            if (!string.IsNullOrEmpty(request.Notes))
                photoDelivery.Notes = request.Notes;

            await _photoDeliveryRepository.UpdatePhotoDeliveryAsync(photoDelivery);
            await _photoDeliveryRepository.SaveChangesAsync();

            var result = await _photoDeliveryRepository.GetPhotoDeliveryByIdAsync(photoDeliveryId);
            return _mapper.Map<PhotoDeliveryResponse>(result);
        }

        public async Task<bool> DeletePhotoDeliveryAsync(int photoDeliveryId)
        {
            var photoDelivery = await _photoDeliveryRepository.GetPhotoDeliveryByIdAsync(photoDeliveryId);
            if (photoDelivery == null)
            {
                return false;
            }

            await _photoDeliveryRepository.DeletePhotoDeliveryAsync(photoDelivery);
            await _photoDeliveryRepository.SaveChangesAsync();
            return true;
        }

        public async Task<PhotoDeliveryResponse> UploadPhotosAsync(UploadPhotosRequest request)
        {
            var photoDelivery = await _photoDeliveryRepository.GetPhotoDeliveryByBookingIdAsync(request.BookingId);
            if (photoDelivery == null)
            {
                throw new InvalidOperationException($"Photo delivery not found for booking {request.BookingId}");
            }

            if (photoDelivery.DeliveryMethod != "PhotographerDevice")
            {
                throw new InvalidOperationException("Photo upload is only allowed for PhotographerDevice delivery method");
            }

            // Validate drive link
            if (!await ValidateDriveLinkAsync(request.DriveLink))
            {
                throw new InvalidOperationException("Invalid Google Drive link provided");
            }

            // Update photo delivery
            photoDelivery.DriveLink = request.DriveLink;
            photoDelivery.DriveFolderName = request.DriveFolderName;
            photoDelivery.PhotoCount = request.PhotoCount;
            photoDelivery.Status = "Delivered";
            photoDelivery.UploadedAt = DateTime.UtcNow;
            photoDelivery.DeliveredAt = DateTime.UtcNow;
            photoDelivery.ExpiresAt = DateTime.UtcNow.AddDays(30); // Link expires in 30 days
            photoDelivery.Notes = request.Notes;

            await _photoDeliveryRepository.UpdatePhotoDeliveryAsync(photoDelivery);
            await _photoDeliveryRepository.SaveChangesAsync();

            // Send notification to customer
            await SendPhotoDeliveryNotificationAsync(request.BookingId, request.DriveLink);

            var result = await _photoDeliveryRepository.GetPhotoDeliveryByIdAsync(photoDelivery.PhotoDeliveryId);
            return _mapper.Map<PhotoDeliveryResponse>(result);
        }

        public async Task<IEnumerable<PhotoDeliveryResponse>> GetPhotoDeliveriesByPhotographerIdAsync(int photographerId)
        {
            var photoDeliveries = await _photoDeliveryRepository.GetPhotoDeliveriesByPhotographerIdAsync(photographerId);
            return _mapper.Map<IEnumerable<PhotoDeliveryResponse>>(photoDeliveries);
        }

        public async Task<IEnumerable<PhotoDeliveryResponse>> GetPendingPhotoDeliveriesForPhotographerAsync(int photographerId)
        {
            var photoDeliveries = await _photoDeliveryRepository.GetPhotoDeliveriesByPhotographerIdAsync(photographerId);
            var pendingDeliveries = photoDeliveries.Where(pd => pd.Status == "Pending" && pd.DeliveryMethod == "PhotographerDevice");
            return _mapper.Map<IEnumerable<PhotoDeliveryResponse>>(pendingDeliveries);
        }

        public async Task<IEnumerable<PhotoDeliveryResponse>> GetPhotoDeliveriesByCustomerIdAsync(int customerId)
        {
            var photoDeliveries = await _photoDeliveryRepository.GetPhotoDeliveriesByCustomerIdAsync(customerId);
            return _mapper.Map<IEnumerable<PhotoDeliveryResponse>>(photoDeliveries);
        }

        public async Task<PhotoDeliveryResponse> MarkAsDeliveredAsync(MarkAsDeliveredRequest request)
        {
            var photoDelivery = await _photoDeliveryRepository.GetPhotoDeliveryByBookingIdAsync(request.BookingId);
            if (photoDelivery == null)
            {
                throw new InvalidOperationException($"Photo delivery not found for booking {request.BookingId}");
            }

            photoDelivery.Status = "Delivered";
            photoDelivery.DeliveredAt = DateTime.UtcNow;
            photoDelivery.Notes = request.Notes;

            await _photoDeliveryRepository.UpdatePhotoDeliveryAsync(photoDelivery);
            await _photoDeliveryRepository.SaveChangesAsync();

            var result = await _photoDeliveryRepository.GetPhotoDeliveryByIdAsync(photoDelivery.PhotoDeliveryId);
            return _mapper.Map<PhotoDeliveryResponse>(result);
        }

        public async Task<IEnumerable<PhotoDeliveryResponse>> GetPhotoDeliveriesByStatusAsync(string status)
        {
            var photoDeliveries = await _photoDeliveryRepository.GetPhotoDeliveriesByStatusAsync(status);
            return _mapper.Map<IEnumerable<PhotoDeliveryResponse>>(photoDeliveries);
        }

        public async Task<IEnumerable<PhotoDeliveryResponse>> GetPendingPhotoDeliveriesAsync()
        {
            var photoDeliveries = await _photoDeliveryRepository.GetPendingPhotoDeliveriesAsync();
            return _mapper.Map<IEnumerable<PhotoDeliveryResponse>>(photoDeliveries);
        }

        public async Task<IEnumerable<PhotoDeliveryResponse>> GetExpiredPhotoDeliveriesAsync()
        {
            var photoDeliveries = await _photoDeliveryRepository.GetExpiredPhotoDeliveriesAsync();
            return _mapper.Map<IEnumerable<PhotoDeliveryResponse>>(photoDeliveries);
        }

        public async Task<bool> IsPhotoDeliveryRequiredAsync(int bookingId)
        {
            var photoDelivery = await _photoDeliveryRepository.GetPhotoDeliveryByBookingIdAsync(bookingId);
            return photoDelivery?.DeliveryMethod == "PhotographerDevice" && photoDelivery?.Status == "Pending";
        }

        public async Task<string> GenerateDriveLinkAsync(int bookingId)
        {
            // This would integrate with Google Drive API to create a folder and return the link
            // For now, return a placeholder
            return $"https://drive.google.com/drive/folders/booking_{bookingId}_{DateTime.UtcNow:yyyyMMddHHmmss}";
        }

        public async Task<bool> ValidateDriveLinkAsync(string driveLink)
        {
            // Basic validation for Google Drive link format
            return !string.IsNullOrEmpty(driveLink) && 
                   (driveLink.Contains("drive.google.com") || driveLink.Contains("docs.google.com"));
        }

        public async Task SendPhotoDeliveryNotificationAsync(int bookingId, string driveLink)
        {
            // This would integrate with notification service to send email/SMS to customer
            // For now, just log the action
            Console.WriteLine($"Photo delivery notification sent for booking {bookingId} with link: {driveLink}");
        }
    }
} 