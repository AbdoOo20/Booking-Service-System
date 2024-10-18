using Microsoft.AspNetCore.SignalR;

namespace BookingServices.Hubs
{
    public class AdminNotification : Hub
    {
        public async Task SendMessage(string user, string message , int length)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message , length);
        }
    }
}
