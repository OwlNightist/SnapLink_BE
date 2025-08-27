using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using SnapLink_Service.IService;
using SnapLink_Repository.DBContext;
using SnapLink_Model.DTO;

namespace SnapLink_API.Jobs
{
    public class BookingAutoCompletionJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BookingAutoCompletionJob> _logger;
        private readonly BookingAutoCompletionSettings _settings;
        private readonly TimeSpan _interval;

        public BookingAutoCompletionJob(
            IServiceProvider serviceProvider, 
            ILogger<BookingAutoCompletionJob> logger,
            IOptions<BookingAutoCompletionSettings> settings)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _settings = settings.Value;
            _interval = TimeSpan.FromHours(_settings.CheckIntervalHours);
            
            _logger.LogInformation("BookingAutoCompletionJob initialized - Enabled: {Enabled}, Check interval: {CheckInterval} hours, Days after booking: {DaysAfter}", 
                _settings.Enabled, _settings.CheckIntervalHours, _settings.DaysAfterBookingEnd);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Wait for the app to fully start
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            // Check if the service is enabled
            if (!_settings.Enabled)
            {
                _logger.LogInformation("BookingAutoCompletionJob is disabled in configuration");
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                    var context = scope.ServiceProvider.GetRequiredService<SnaplinkDbContext>();

                    // Get bookings that are confirmed and X days past the booking end date (configurable)
                    var cutoffDate = DateTime.UtcNow.AddDays(-_settings.DaysAfterBookingEnd);
                    
                    var bookingsToComplete = await context.Bookings
                        .Include(b => b.PhotoDelivery)
                        .Where(b => b.Status == "Confirmed" && 
                                   b.EndDatetime.HasValue && 
                                   b.EndDatetime.Value <= cutoffDate)
                        .ToListAsync(stoppingToken);

                    int completedCount = 0;

                    foreach (var booking in bookingsToComplete)
                    {
                        bool shouldComplete = false;

                        if (booking.PhotoDelivery != null)
                        {
                            // Check the delivery method criteria
                            if (booking.PhotoDelivery.DeliveryMethod == "CustomerDevice")
                            {
                                // If delivery method is CustomerDevice, auto-complete
                                shouldComplete = true;
                                _logger.LogInformation("Auto-completing booking {BookingId} - DeliveryMethod is CustomerDevice", booking.BookingId);
                            }
                            else if (booking.PhotoDelivery.DeliveryMethod == "PhotographerDevice")
                            {
                                // If delivery method is PhotographerDevice and has DriveLink, auto-complete
                                if (!string.IsNullOrWhiteSpace(booking.PhotoDelivery.DriveLink))
                                {
                                    shouldComplete = true;
                                    _logger.LogInformation("Auto-completing booking {BookingId} - DeliveryMethod is PhotographerDevice with DriveLink", booking.BookingId);
                                }
                            }
                        }
                        else
                        {
                            // If no PhotoDelivery record exists, auto-complete (assuming photos were delivered physically or through other means)
                            shouldComplete = true;
                            _logger.LogInformation("Auto-completing booking {BookingId} - No PhotoDelivery record found", booking.BookingId);
                        }

                        if (shouldComplete)
                        {
                            try
                            {
                                var result = await bookingService.CompleteBookingAsync(booking.BookingId);
                                if (result.Error == 0)
                                {
                                    completedCount++;
                                    _logger.LogInformation("Successfully auto-completed booking {BookingId}", booking.BookingId);
                                }
                                else
                                {
                                    _logger.LogWarning("Failed to auto-complete booking {BookingId}: {ErrorMessage}", 
                                        booking.BookingId, result.Message);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error auto-completing booking {BookingId}", booking.BookingId);
                            }
                        }
                        else
                        {
                            _logger.LogDebug("Booking {BookingId} not eligible for auto-completion - DeliveryMethod: {DeliveryMethod}, DriveLink: {HasDriveLink}", 
                                booking.BookingId, 
                                booking.PhotoDelivery?.DeliveryMethod ?? "N/A",
                                !string.IsNullOrWhiteSpace(booking.PhotoDelivery?.DriveLink) ? "Yes" : "No");
                        }
                    }

                    if (completedCount > 0)
                    {
                        _logger.LogInformation("Auto-completed {CompletedCount} bookings out of {TotalEligible} eligible bookings", 
                            completedCount, bookingsToComplete.Count);
                    }
                    else if (bookingsToComplete.Any())
                    {
                        _logger.LogInformation("Found {TotalEligible} bookings past 3 days but none were eligible for auto-completion", 
                            bookingsToComplete.Count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "BookingAutoCompletionJob failed during execution");
                }

                // Wait before next check
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
