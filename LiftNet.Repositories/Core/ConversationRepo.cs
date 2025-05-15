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
    public class ConversationRepo : CrudBaseRepo<Conversation>, IConversationRepo
    {
        public ConversationRepo(LiftNetDbContext dbContext, ILiftLogger<CrudBaseRepo<Conversation>> logger) : base(dbContext, logger)
        {
        }
    }
}
