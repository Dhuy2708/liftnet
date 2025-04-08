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
        public DateTime StartTime
        {
            get; set;
        }
        public DateTime EndTime
        {
            get; set;
        }
        public AppointmentStatus Status
        {
            get; set;
        }
        public RepeatingType RepeatingType
        {
            get; set;
        }
        public DateTime Created
        {
            get; set;
        }
        public DateTime Modified
        {
            get; set;
        }
    }
}
