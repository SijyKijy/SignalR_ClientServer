using Server.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System;

namespace SignalR_Test.Hubs
{
    public class TestHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"User {Context.ConnectionId} connected");
            Clients.All.SendAsync("Connected", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"User {Context.ConnectionId} disconnected");
            return base.OnDisconnectedAsync(exception);
        }

        public async Task Send(Message message) =>
            await Clients.All.SendAsync("Recive", message);
    }
}
