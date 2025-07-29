using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SnapLink_Model.DTO.Request
{
    public class CreatePaymentLinkRequest
    {
        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int BookingId { get; set; }

        // Thêm các trường cho phép frontend truyền vào
        public string? SuccessUrl { get; set; }
        public string? CancelUrl { get; set; }
    }
}