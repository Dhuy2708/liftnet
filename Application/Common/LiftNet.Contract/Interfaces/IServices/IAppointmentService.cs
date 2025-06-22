using LiftNet.Contract.Dtos.Appointment;
using LiftNet.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IServices
{
    public interface IAppointmentService : IDependency
    {
        Task<List<AppointmentDto>> ListUserAppointments(string userId);
        [Obsolete] Task<int> FeedBackAsync(AppointmentFeedbackRequestDto request);
        Task PingAppointmentNotiCount(string appointmentId, string factorUserId);
    }
}
