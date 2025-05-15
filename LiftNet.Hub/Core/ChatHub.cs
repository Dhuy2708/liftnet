using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Hub.Constant;
using LiftNet.Hub.Contract;
using LiftNet.Hub.Provider;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Hub.Core
{
    [Authorize]
    public class ChatHub : BaseHub<ChatMessage>
    {
        private readonly I _uow;
        private readonly ICh

        public ChatHub(ConnectionPool connPool) : base(connPool, HubNames.chat)
        {
        }

        public async Task SendMessage(string userId, ChatMessage message)
        {
            await SendToUser(userId, message);


        }
    }
}
