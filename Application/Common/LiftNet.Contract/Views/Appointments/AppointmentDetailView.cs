using LiftNet.Contract.Dtos;
using LiftNet.Contract.Enums.Appointment;
using LiftNet.Contract.Views.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Appointments
{
    public class AppointmentDetailView
    {
        public string Id
        {
            get; set;
        }
        public bool Editable
        {
            get; set;
        }
        public UserViewInAppointmentDetail Booker
        {
            get; set;
        }
        public List<UserViewInAppointmentDetail> OtherParticipants
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

    public class UserViewInAppointmentDetail : UserOverview
    {
        public AppointmentParticipantStatus Status
        {
            get; set;
        }
    }
}
