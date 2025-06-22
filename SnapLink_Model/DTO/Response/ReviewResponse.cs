using System;

namespace SnapLink_Model.DTO.Response
{
    public class ReviewResponse
    {
        public int ReviewId { get; set; }
        public int BookingId { get; set; }
        public int ReviewerId { get; set; }
        public int RevieweeId { get; set; }
        public string? RevieweeType { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Related data
        public string? ReviewerName { get; set; }
        public string? RevieweeName { get; set; }
        public string? BookingDescription { get; set; }
    }
} 