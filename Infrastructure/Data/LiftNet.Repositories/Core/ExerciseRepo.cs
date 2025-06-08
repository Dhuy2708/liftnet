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
    public class ExerciseRepo : CrudBaseRepo<Exercise>, IExerciseRepo
    {
        public ExerciseRepo(LiftNetDbContext dbContext, ILiftLogger<CrudBaseRepo<Exercise>> logger) : base(dbContext, logger)
        {
        }
    }
}
