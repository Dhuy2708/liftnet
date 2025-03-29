using LiftNet.Contract.Enums;
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
        public UserOverview Booker
        {
            get; set;
        }
        public List<UserOverview> Participants
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
        public AddressView Address
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
