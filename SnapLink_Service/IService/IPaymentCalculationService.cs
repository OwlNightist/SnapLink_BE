using SnapLink_Repository.Entity;

namespace SnapLink_Service.IService
{
    public interface IPaymentCalculationService
    {
        Task<PaymentCalculationResult> CalculatePaymentDistributionAsync(decimal totalAmount, int? locationId, int bookingid);
        Task<PaymentCalculationResult> CalculatePaymentDistributionAsync(decimal totalAmount, Location? location, int bookingid);
        decimal CalculatePlatformFee(decimal totalAmount);
        decimal CalculateLocationFee(Location? location, EventBooking? eventBooking = null);
        decimal CalculatePhotographerPayout(decimal totalAmount, decimal platformFee, decimal locationFee);
    }

    public class PaymentCalculationResult
    {
        public decimal TotalAmount { get; set; }
        public decimal PlatformFee { get; set; }
        public decimal LocationFee { get; set; }
        public decimal PhotographerPayout { get; set; }
        public decimal PlatformFeePercentage { get; set; }
        public string LocationType { get; set; }
        public string CalculationNote { get; set; }
    }
} 