using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Hub.Contract;
using LiftNet.Hub.Provider;
using LiftNet.Utility.Extensions;
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
        protected string CallerId => Context.UserIdentifier ?? string.Empty;

        public BaseHub(ConnectionPool connPool, string hubName)
        {
            _connPool = connPool;
            _hubName = hubName;
        }

        public Task Ping() => Task.CompletedTask;

        public override async Task OnConnectedAsync()
        {
            if (!string.IsNullOrEmpty(CallerId))
            {
                _connPool.AddConnection(CallerId, Context.ConnectionId, _hubName);
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

        protected async Task SendToCaller(T message, string? methodName = null)
        {
            var userId = Context.UserIdentifier;
            if (userId.IsNullOrEmpty())
            {
                return;
            }

            var connections = _connPool.GetUserConnectionsByHub(userId!, _hubName);
            await Send(connections, message, methodName);
        }

        protected async Task SendToCaller(object message, string? methodName = null)
        {
            var userId = Context.UserIdentifier;
            if (userId.IsNullOrEmpty())
            {
                return;
            }

            var connections = _connPool.GetUserConnectionsByHub(userId!, _hubName);
            await Send(connections, message, methodName);
        }

        protected async Task SendToUser(string userId, T message, string? methodName = null)
        {
            var connections = _connPool.GetUserConnectionsByHub(userId, _hubName);
            await Send(connections, message, methodName);
        }

        protected async Task SendToUsers(List<string> userIds, T message)
        {
            var tasks = userIds.Select(x =>
            {
                var connections = _connPool.GetUserConnectionsByHub(x, _hubName);
                return Send(connections, message);
            });
            await Task.WhenAll(tasks);
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

        protected async Task Send(IEnumerable<string> connections, T message, string? methodName = null)
        {
            if (!connections.Any())
            {
                return;
            }
            methodName ??= "RecieveMessage";
            var tasks = connections.Select(connectionId =>
                                                   Clients.Client(connectionId).SendAsync(methodName, message)
                                                ).ToList();
            await Task.WhenAll(tasks);
        }

        protected async Task Send(IEnumerable<string> connections, object message, string? methodName = null)
        {
            if (!connections.Any())
            {
                return;
            }
            methodName ??= "RecieveMessage";
            var tasks = connections.Select(connectionId =>
                                                   Clients.Client(connectionId).SendAsync(methodName, message)
                                                ).ToList();
            await Task.WhenAll(tasks);
        }
    }
}
