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
    public class FinderPostSeenStatusRepo : CrudBaseRepo<FinderPostSeenStatus>, IFinderPostSeenStatusRepo
    {
        public FinderPostSeenStatusRepo(LiftNetDbContext dbContext, ILiftLogger<CrudBaseRepo<FinderPostSeenStatus>> logger) : base(dbContext, logger)
        {
        }
    }
}
