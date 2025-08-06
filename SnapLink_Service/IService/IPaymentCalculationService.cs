using SnapLink_Repository.Entity;

namespace SnapLink_Service.IService
{
    public interface IPaymentCalculationService
    {
        Task<PaymentCalculationResult> CalculatePaymentDistributionAsync(decimal totalAmount, int? locationId);
        Task<PaymentCalculationResult> CalculatePaymentDistributionAsync(decimal totalAmount, Location location);
        decimal CalculatePlatformFee(decimal totalAmount);
        decimal CalculateLocationFee(Location location);
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