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

    public class CreateWalletTopUpRequest
    {
        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(5000, 10000000, ErrorMessage = "Amount must be between 5,000 and 10,000,000 VND")]
        public decimal Amount { get; set; }

        [Required]
        public string SuccessUrl { get; set; } = string.Empty;

        [Required]
        public string CancelUrl { get; set; } = string.Empty;
    }
}