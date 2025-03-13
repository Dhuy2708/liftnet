using LiftNet.Contract.Dtos;
using LiftNet.Contract.Enums;

namespace LiftNet.Contract.Views.Appointments
{
    public class AppointmentView
    {
        public string Id
        {
            get; set;
        }
        public UserView Client
        {
            get; set;
        }
        public UserView Coach
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
    }
}
