using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SalesMonitor.Data;
using SalesMonitor.Hubs;
using SalesMonitor.Models;

namespace SalesMonitor.Services
{
    public class ChurnMonitorService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly IHubContext<AlertHub> _hub;
        private readonly ILogger<ChurnMonitorService> _logger;

        public ChurnMonitorService(IServiceProvider services, IHubContext<AlertHub> hub, ILogger<ChurnMonitorService> logger)
        {
            _services = services;
            _hub = hub;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var activities = await db.ClientActivities
                        .Include(a => a.Product)
                        .Where(a => a.LastSaleDate != null)
                        .ToListAsync(stoppingToken);

                    foreach (var activity in activities)
                    {
                        var daysSinceLastSale = (DateTime.Today - activity.LastSaleDate!.Value).TotalDays;
                        var threshold = (double)(activity.AverageOrderInterval * 2.5m);

                        if (daysSinceLastSale > threshold && activity.AverageOrderInterval > 0)
                        {
                            await _hub.Clients.All.SendAsync("ReceiveAlert", new AlertNotification
                            {
                                ProductName = activity.Product.Name,
                                IssueType = "Churn",
                                Message = $"Риск ухода! Дней без продаж: {daysSinceLastSale:F0} (норма: {activity.AverageOrderInterval:F1})"
                            }, stoppingToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка мониторинга");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}