using LiftNet.Contract.Dtos;
using LiftNet.Hub.Constant;
using LiftNet.Hub.Contract;
using LiftNet.Utility.Extensions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Hub.Core
{
    [Authorize]
    public class NotiHub : BaseHub<NotiMessage>
    {
        public NotiHub(ConnectionPool connPool) : base(connPool, HubNames.noti)
        {
        }

        public async Task SendNotiToOneUser(NotiMessage message)
        {
            if (message.RecieverId.IsNullOrEmpty())
            {
                return;
            }
            await SendToUser(message.RecieverId!, message, "ReceiveNoti");
        }

        public async Task SendNotiToUsers(List<NotiMessage> messages)
        {
            if (messages.IsNullOrEmpty())
            {
                return;
            }
            var groupedMessages = messages.GroupBy(m => m.RecieverId).ToList();
            var tasks = groupedMessages
                    .Where(group => !group.Key.IsNullOrEmpty())
                    .Select(group =>
                    {
                        var userId = group.Key;
                        var notiMessage = group.First();
                        return SendToUser(userId!, notiMessage, "ReceiveNoti");
                    });

             await Task.WhenAll(tasks);
        }
    }
}
