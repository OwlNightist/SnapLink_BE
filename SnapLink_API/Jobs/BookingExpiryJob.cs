using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SnapLink_Service.IService;

namespace SnapLink_API.Jobs
{
	public class BookingExpiryJob : BackgroundService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<BookingExpiryJob> _logger;
		private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

		public BookingExpiryJob(IServiceProvider serviceProvider, ILogger<BookingExpiryJob> logger)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			// Wait for the app to fully start
			await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					using var scope = _serviceProvider.CreateScope();
					var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
					var cancelled = await bookingService.CancelExpiredPendingBookingsAsync();
					if (cancelled > 0)
					{
						_logger.LogInformation("Cancelled {Count} expired pending bookings.", cancelled);
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "BookingExpiryJob failed.");
				}

				await Task.Delay(_interval, stoppingToken);
			}
		}
	}
}


