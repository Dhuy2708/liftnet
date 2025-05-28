using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Repositories.Core
{
    public class ChatBotConversationRepo : CrudBaseRepo<ChatBotConversation>, IChatBotConversationRepo
    {
        public ChatBotConversationRepo(LiftNetDbContext dbContext, ILiftLogger<CrudBaseRepo<ChatBotConversation>> logger) : base(dbContext, logger)
        {
        }
    }
}
