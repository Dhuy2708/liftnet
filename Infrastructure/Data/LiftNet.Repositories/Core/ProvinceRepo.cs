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
    internal class ProvinceRepo : CrudBaseRepo<Province>, IProvinceRepo
    {
        public ProvinceRepo(LiftNetDbContext dbContext, ILiftLogger<CrudBaseRepo<Province>> logger) : base(dbContext, logger)
        {
        }
    }
}
