using Microsoft.AspNetCore.SignalR;
namespace Messaging_WebApp.Hubs
{
    public class MyHub : Hub
    {
        public async Task Changed( string contName, string message, string username)
        {
            await Clients.All.SendAsync("ChangeReceived", contName, message, username);
        }
    }
}
