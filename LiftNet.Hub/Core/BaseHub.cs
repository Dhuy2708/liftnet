using LiftNet.Hub.Contract;
using LiftNet.Hub.Provider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Hub.Core
{
    [Authorize]
    public class BaseHub<T> : Microsoft.AspNetCore.SignalR.Hub
                   where T : HubMessage
    {
        protected readonly ConnectionPool _connPool;
        protected string _hubName;

        public BaseHub(ConnectionPool connPool, string hubName)
        {
            _connPool = connPool;
            _hubName = hubName;
        }

        public Task Ping() => Task.CompletedTask;

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                _connPool.AddConnection(userId, Context.ConnectionId, _hubName);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.Sid)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _connPool.RemoveConnection(userId, Context.ConnectionId, _hubName);
            }
            await base.OnDisconnectedAsync(exception);
        }


        protected async Task SendToUser(string userId, T message)
        {
            var connections = _connPool.GetUserConnectionsByHub(userId, _hubName);
            await Send(connections, message);
        }

        protected async Task SendToAllInOneHub(T message)
        {
            var conns = _connPool.GetAllConnectionsByHub(_hubName);
            await Send(conns, message);
        }

        protected async Task SendToAll(T message)
        {
            await Clients.All.SendAsync("RecieveMessage", message);
        }

        protected async Task Send(IEnumerable<string> connections, T message)
        {
            if (!connections.Any())
            {
                return;
            }

            var tasks = connections.Select(connectionId =>
                                                   Clients.Client(connectionId).SendAsync("RecieveMessage", message)
                                                ).ToList();
            await Task.WhenAll(tasks);
        }
    }
}
