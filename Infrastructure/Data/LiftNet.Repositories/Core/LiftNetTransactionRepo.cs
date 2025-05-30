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
    public class LiftNetTransactionRepo : CrudBaseRepo<LiftNetTransaction>, ILiftNetTransactionRepo
    {
        public LiftNetTransactionRepo(LiftNetDbContext dbContext, ILiftLogger<CrudBaseRepo<LiftNetTransaction>> logger) : base(dbContext, logger)
        {
        }
    }
}
