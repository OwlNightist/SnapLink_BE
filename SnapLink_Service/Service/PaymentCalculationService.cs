using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SnapLink_Repository.DBContext;
using SnapLink_Repository.Entity;
using SnapLink_Service.IService;

namespace SnapLink_Service.Service
{
    public class PaymentCalculationService : IPaymentCalculationService
    {
        private readonly SnaplinkDbContext _context;
        private readonly IConfiguration _configuration;

        public PaymentCalculationService(SnaplinkDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<PaymentCalculationResult> CalculatePaymentDistributionAsync(decimal totalAmount, int? locationId)
        {
            Location? location = null;
            
            if (locationId.HasValue)
            {
                location = await _context.Locations
                    .FirstOrDefaultAsync(l => l.LocationId == locationId.Value);
            }

            return await CalculatePaymentDistributionAsync(totalAmount, location);
        }

        public async Task<PaymentCalculationResult> CalculatePaymentDistributionAsync(decimal totalAmount, Location? location)
        {
            var platformFeePercentage = _configuration.GetValue<decimal>("PaymentSettings:PlatformFeePercentage", 10);
            var platformFee = CalculatePlatformFee(totalAmount);
            var locationFee = CalculateLocationFee(location);
            var photographerPayout = CalculatePhotographerPayout(totalAmount, platformFee, locationFee);

            var locationType = location?.LocationType ?? "External";
            var calculationNote = GenerateCalculationNote(totalAmount, platformFee, locationFee, photographerPayout, locationType);

            return new PaymentCalculationResult
            {
                TotalAmount = totalAmount,
                PlatformFee = platformFee,
                LocationFee = locationFee,
                PhotographerPayout = photographerPayout,
                PlatformFeePercentage = platformFeePercentage,
                LocationType = locationType,
                CalculationNote = calculationNote
            };
        }

        public decimal CalculatePlatformFee(decimal totalAmount)
        {
            var platformFeePercentage = _configuration.GetValue<decimal>("PaymentSettings:PlatformFeePercentage", 10);
            return totalAmount * (platformFeePercentage / 100m);
        }

        public decimal CalculateLocationFee(Location? location)
        {
            if (location == null)
                return 0m;

            // Only registered locations have fees
            if (location.LocationType == "Registered" || location.LocationType == null)
            {
                return location.HourlyRate ?? 0;
            }

            // External locations (Google Places) have no fee
            return 0m;
        }

        public decimal CalculatePhotographerPayout(decimal totalAmount, decimal platformFee, decimal locationFee)
        {
            return totalAmount - platformFee - locationFee;
        }

        private string GenerateCalculationNote(decimal totalAmount, decimal platformFee, decimal locationFee, decimal photographerPayout, string locationType)
        {
            var platformFeePercentage = _configuration.GetValue<decimal>("PaymentSettings:PlatformFeePercentage", 10);
            
            var note = $"Payment breakdown: Total=${totalAmount}, Platform Fee (${platformFeePercentage}%)=${platformFee}";
            
            if (locationFee > 0)
            {
                note += $", Location Fee (${locationType})=${locationFee}";
            }
            else
            {
                note += $", Location Fee (${locationType})=$0";
            }
            
            note += $", Photographer Payout=${photographerPayout}";
            
            return note;
        }
    }
} 