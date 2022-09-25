using Microsoft.AspNetCore.SignalR;

namespace Projeto.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user,string receiver, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, receiver, message);
        }
    }
}