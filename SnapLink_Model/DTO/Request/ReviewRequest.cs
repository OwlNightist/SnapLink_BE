using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Request
{
    public class CreateReviewRequest
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        public int ReviewerId { get; set; }

        [Required]
        public int RevieweeId { get; set; }

        [Required]
        [MaxLength(30)]
        public string RevieweeType { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }

    public class UpdateReviewRequest
    {
        [Range(1, 5)]
        public int? Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }
} 