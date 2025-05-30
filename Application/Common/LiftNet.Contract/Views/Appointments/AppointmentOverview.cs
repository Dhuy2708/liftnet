using LiftNet.Contract.Dtos;
using LiftNet.Contract.Enums.Appointment;
using LiftNet.Contract.Views.Users;

namespace LiftNet.Contract.Views.Appointments
{
    public class AppointmentOverview
    {
        public string Id
        {
            get; set;
        }
        public UserOverview Booker
        {
            get; set;
        }
        public int AcceptedCount
        {
            get; set;
        }
        public int ParticipantCount
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
        public PlaceDetailDto? Location
        {
            get; set;
        }
        public DateTimeOffset StartTime
        {
            get; set;
        }
        public DateTimeOffset EndTime
        {
            get; set;
        }
        public AppointmentParticipantStatus Status
        {
            get; set;
        }
        public AppointmentStatus AppointmentStatus
        {
            get; set;
        }
        public RepeatingType RepeatingType
        {
            get; set;
        }
        public DateTimeOffset Created
        {
            get; set;
        }
        public DateTimeOffset Modified
        {
            get; set;
        }
    }
}
