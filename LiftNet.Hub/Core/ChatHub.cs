using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices.Indexes;
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
        private readonly IChatIndexService _chatService;

        public ChatHub(ConnectionPool connPool, IChatIndexService chatService) : base(connPool, HubNames.chat)
        {
            _chatService = chatService;
        }

        public async Task SendMessage(string userId, ChatMessage message)
        {
            await SendToUser(userId, message);
        }
    }
}
