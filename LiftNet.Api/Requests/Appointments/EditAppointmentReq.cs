using LiftNet.Contract.Enums;
using System.Diagnostics.CodeAnalysis;

namespace LiftNet.Api.Requests.Appointments
{
    public class EditAppointmentReq
    {
        [NotNull]
        public string AppointmentId
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public string Description
        {
            get; set;
        }
        public DateTime? StartTime
        {
            get; set;
        }
        public DateTime? EndTime
        {
            get; set;
        }
        public RepeatingType RepeatingType
        {
            get; set;
        }
        public List<string> ParticipantIds
        {
            get; set;
        }
    }
}
