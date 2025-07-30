using System;
using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Request
{
    public class CreatePhotoDeliveryRequest
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        [MaxLength(50)]
        public string DeliveryMethod { get; set; } = string.Empty; // "CustomerDevice" or "PhotographerDevice"

        [MaxLength(500)]
        public string? DriveLink { get; set; } // Google Drive link

        [MaxLength(100)]
        public string? DriveFolderName { get; set; } // Name of the folder in Drive

        public int? PhotoCount { get; set; } // Number of photos delivered

        [MaxLength(500)]
        public string? Notes { get; set; } // Additional notes
    }

    public class UpdatePhotoDeliveryRequest
    {
        [MaxLength(500)]
        public string? DriveLink { get; set; }

        [MaxLength(100)]
        public string? DriveFolderName { get; set; }

        public int? PhotoCount { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }

        public DateTime? ExpiresAt { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    public class UploadPhotosRequest
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        [MaxLength(500)]
        public string DriveLink { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? DriveFolderName { get; set; }

        public int? PhotoCount { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    public class MarkAsDeliveredRequest
    {
        [Required]
        public int BookingId { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
} 