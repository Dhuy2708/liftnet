using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Ioc;
using LiftNet.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Repositories.Core
{
    internal class SocialConnectionRepo : CrudBaseRepo<SocialConnection>, ISocialConnectionRepo, IDependency
    {
        public SocialConnectionRepo(LiftNetDbContext dbContext, ILiftLogger<CrudBaseRepo<SocialConnection>> logger) : base(dbContext, logger)
        {
        }
    }
}
