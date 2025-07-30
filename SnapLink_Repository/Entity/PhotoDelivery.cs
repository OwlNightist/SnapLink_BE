using System;
using System.ComponentModel.DataAnnotations;

namespace SnapLink_Repository.Entity;

public partial class PhotoDelivery
{
    public int PhotoDeliveryId { get; set; }

    public int BookingId { get; set; }

    [Required]
    [MaxLength(50)]
    public string DeliveryMethod { get; set; } = string.Empty; // "CustomerDevice" or "PhotographerDevice"

    [MaxLength(500)]
    public string? DriveLink { get; set; } // Google Drive link

    [MaxLength(100)]
    public string? DriveFolderName { get; set; } // Name of the folder in Drive

    public int? PhotoCount { get; set; } // Number of photos delivered

    [MaxLength(20)]
    public string Status { get; set; } = "Pending"; // "Pending", "Uploading", "Delivered", "NotRequired"

    public DateTime? UploadedAt { get; set; } // When photographer uploaded to Drive

    public DateTime? DeliveredAt { get; set; } // When customer received the link

    public DateTime? ExpiresAt { get; set; } // Link expiration date

    [MaxLength(500)]
    public string? Notes { get; set; } // Additional notes

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Booking Booking { get; set; } = null!;
} 