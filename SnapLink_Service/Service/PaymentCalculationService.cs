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

        public async Task<PaymentCalculationResult> CalculatePaymentDistributionAsync(decimal totalAmount, int? locationId,int bookingid)
        {
            Location? location = null;
            
            if (locationId.HasValue)
            {
                location = await _context.Locations
                    .FirstOrDefaultAsync(l => l.LocationId == locationId.Value);
            }

            return await CalculatePaymentDistributionAsync(totalAmount, location, bookingid);
        }

        public async Task<PaymentCalculationResult> CalculatePaymentDistributionAsync(decimal totalAmount, Location? location, int bookingid)
        {
            var platformFeePercentage = _configuration.GetValue<decimal>("PaymentSettings:PlatformFeePercentage", 10);
            var platformFee = CalculatePlatformFee(totalAmount);
            
            // Check if this is an event booking
            var eventBooking = await _context.EventBookings
                .Include(eb => eb.EventPhotographer)
                .FirstOrDefaultAsync(eb => eb.BookingId == bookingid);
            
            var locationFee = CalculateLocationFee(location, eventBooking);
            var truelocationFewithplatformFee = locationFee - locationFee * (platformFeePercentage / 100m);
            var photographerPayout = CalculatePhotographerPayout(totalAmount, platformFee, truelocationFewithplatformFee);

            var locationType = location?.LocationType ?? "External";
            var calculationNote = GenerateCalculationNote(totalAmount, platformFee, truelocationFewithplatformFee, photographerPayout, locationType, eventBooking);

            return new PaymentCalculationResult
            {
                TotalAmount = totalAmount,
                PlatformFee = platformFee,
                LocationFee = truelocationFewithplatformFee,
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

        public decimal CalculateLocationFee(Location? location, EventBooking? eventBooking = null)
        {
            // If this is an event booking, use EventPrice instead of Location.HourlyRate
            if (eventBooking != null)
            {
                return eventBooking.EventPrice;
            }

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

        private string GenerateCalculationNote(decimal totalAmount, decimal platformFee, decimal locationFee, decimal photographerPayout, string locationType, EventBooking? eventBooking = null)
        {
            var platformFeePercentage = _configuration.GetValue<decimal>("PaymentSettings:PlatformFeePercentage", 10);
            
            var note = $"Payment breakdown: Total=${totalAmount}, Platform Fee (${platformFeePercentage}%)=${platformFee}";
            
            if (eventBooking != null)
            {
                note += $", Event Price=${locationFee}";
            }
            else if (locationFee > 0)
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