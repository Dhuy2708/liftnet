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
    internal class DistrictRepo : CrudBaseRepo<District>, IDistrictRepo
    {
        public DistrictRepo(LiftNetDbContext dbContext, ILiftLogger<CrudBaseRepo<District>> logger) : base(dbContext, logger)
        {
        }
    }
}
