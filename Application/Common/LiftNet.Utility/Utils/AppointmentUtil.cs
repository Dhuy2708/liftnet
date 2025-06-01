using LiftNet.Contract.Enums.Appointment;
using LiftNet.Domain.Entities;
using LiftNet.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Utility.Utils
{
    public static class AppointmentUtil
    {
        public static AppointmentStatus GetAppointmentStatus(Appointment appointment)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null");
            }
            if (appointment.StartTime > DateTime.UtcNow)
            {
                return AppointmentStatus.Upcomming;
            }
            if (appointment.StartTime <= DateTime.UtcNow && appointment.EndTime >= DateTime.UtcNow)
            {
                return AppointmentStatus.InProgress;
            }
            if (appointment.Participants.IsNullOrEmpty())
            {
                throw new InvalidOperationException("participants are null or empty");
            } 

            var allAccept = appointment.Participants.All(x => x.Status == (int)AppointmentParticipantStatus.Accepted);

            if (appointment.EndTime < DateTime.UtcNow)
            {
                if (allAccept)
                {
                    return AppointmentStatus.Finished;
                }
                else
                {
                    return AppointmentStatus.Expired;
                }
            }

            return AppointmentStatus.None;
        }
    }
}
