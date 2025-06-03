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
    public class PhysicalStatRepo : CrudBaseRepo<UserPhysicalStat>, IPhysicalStatRepo
    {
        public PhysicalStatRepo(LiftNetDbContext dbContext, ILiftLogger<CrudBaseRepo<UserPhysicalStat>> logger) : base(dbContext, logger)
        {
        }
    }
}
