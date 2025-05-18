using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Persistence.Context;
using Microsoft.EntityFrameworkCore;
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

        public async Task<bool> IsConversationExist(string conversationId, string userId)
        {
            return await GetQueryable().AnyAsync(x => x.Id == conversationId && (x.UserId1 == userId || x.UserId2 == userId));
        }
    }
}
