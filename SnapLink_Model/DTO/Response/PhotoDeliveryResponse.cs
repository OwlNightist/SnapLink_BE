using System;

namespace SnapLink_Model.DTO.Response
{
    public class PhotoDeliveryResponse
    {
        public int PhotoDeliveryId { get; set; }
        public int BookingId { get; set; }
        public string DeliveryMethod { get; set; } = string.Empty;
        public string? DriveLink { get; set; }
        public string? DriveFolderName { get; set; }
        public int? PhotoCount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? UploadedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Booking information
        public string? CustomerName { get; set; }
        public string? PhotographerName { get; set; }
        public DateTime? BookingDate { get; set; }
        public string? LocationName { get; set; }
    }

    public class PhotoDeliveryDetailResponse : PhotoDeliveryResponse
    {
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public string? PhotographerEmail { get; set; }
        public string? PhotographerPhone { get; set; }
        public string? BookingStatus { get; set; }
        public decimal? TotalPrice { get; set; }
    }

    public class PhotoDeliverySummaryResponse
    {
        public int BookingId { get; set; }
        public string DeliveryMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? DeliveredAt { get; set; }
        public string? DriveLink { get; set; }
        public int? PhotoCount { get; set; }
        public string? CustomerName { get; set; }
        public string? PhotographerName { get; set; }
    }
} 