using LiftNet.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IServices
{
    public interface IChatbotService : IDependency
    {
        Task<string> ChatAsync(string userId, string conversationId, string message);
        Task<bool> CheckConversationAsync(string userId, string conversationId);
    }
}
