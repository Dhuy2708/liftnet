using LiftNet.Contract.Interfaces.Repositories;
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
    internal class UserRepo : CrudBaseRepo<User>, IUserRepo
    {
        public UserRepo(LiftNetDbContext dbContext, ILiftLogger<CrudBaseRepo<User>> logger) : base(dbContext, logger)
        {
        }
    }
}
