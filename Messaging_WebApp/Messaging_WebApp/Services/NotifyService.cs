using Messaging_WebApp.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Messaging_WebApp.Services
{
    public class NotifyService
    {
        private readonly IHubContext<MyHub> _hub;

        public NotifyService(IHubContext<MyHub> hub)
        {
            _hub = hub;
        }

        public Task SendNotificationAsync(string message)
        {
            return _hub.Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
