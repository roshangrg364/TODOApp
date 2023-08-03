using DomainModule.ServiceInterface;
using Microsoft.AspNetCore.SignalR;

namespace TodoApp.SignalR
{
    public class TodoHub : Hub
    {
        
        public readonly static ConnectionMapping<string> _connections =
             new ConnectionMapping<string>();

        public void SendChatMessage(string userId)
        {
            string name = Context.User.Identity.Name;

            foreach (var connectionId in _connections.GetConnections(userId))
            {
                Clients.Client(connectionId).SendAsync("ReceiveMessage","dsfdfdf");
            }
        }

        public override Task OnConnectedAsync()
        {
            string name = Context.User.Identity.Name;

            _connections.Add(name, Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            string name = Context.User.Identity.Name;

            _connections.Remove(name, Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

      
    }
}