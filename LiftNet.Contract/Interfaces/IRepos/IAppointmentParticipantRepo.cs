using LiftNet.Domain.Entities;
using LiftNet.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IRepos
{
    public interface IAppointmentParticipantRepo : ICrudBaseRepo<AppointmentParticipant>, IDependency
    {
    }
}
