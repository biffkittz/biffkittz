using Microsoft.AspNetCore.SignalR;

// TODO: Add logic and play around with SignalR now that it works behind nginx
namespace ListedLinks.Hubs
{
    public class SaaSActivityHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}