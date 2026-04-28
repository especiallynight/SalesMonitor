using Microsoft.AspNetCore.SignalR;

namespace SalesMonitor.Hubs
{
    public class AlertHub : Hub
    {
        public async Task SubscribeToAlerts()
        {
            await Clients.Caller.SendAsync("Subscribed", "Вы подписаны на уведомления");
        }
    }
}