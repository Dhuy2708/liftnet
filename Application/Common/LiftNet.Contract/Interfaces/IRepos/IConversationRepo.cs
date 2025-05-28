
using LiftNet.Domain.Entities;
using LiftNet.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IRepos
{
    public interface IConversationRepo : ICrudBaseRepo<Conversation>, IDependency
    {
        Task<bool> IsConversationExist(string conversationId, string userId);
        Task<bool> IsConversationExistByUserId(string userId1, string userId2);
    }
}
