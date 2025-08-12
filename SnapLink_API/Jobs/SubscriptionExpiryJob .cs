using SnapLink_Service.IService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
namespace SnapLink_API.Jobs
{
    public class SubscriptionExpiryJob : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<SubscriptionExpiryJob> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(15);

        public SubscriptionExpiryJob(IServiceProvider sp, ILogger<SubscriptionExpiryJob> logger)
        {
            _sp = sp; _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // đợi app khởi động xong
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _sp.CreateScope();
                    var svc = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
                    var n = await svc.ExpireOverduesAsync();
                    if (n > 0) _logger.LogInformation("Expired {Count} subscriptions.", n);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "SubscriptionExpiryJob failed.");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
