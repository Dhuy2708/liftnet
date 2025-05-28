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
    public class AppointmentParticipantRepo : CrudBaseRepo<AppointmentParticipant>, IAppointmentParticipantRepo
    {
        public AppointmentParticipantRepo(LiftNetDbContext dbContext, ILiftLogger<CrudBaseRepo<AppointmentParticipant>> logger) : base(dbContext, logger)
        {
        }
    }
}
