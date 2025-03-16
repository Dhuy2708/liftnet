using LiftNet.Contract.Enums;
using LiftNet.Contract.Views;

namespace LiftNet.Api.Requests
{
    public class BookAppointmentReq
    {
        public string TargetUserId
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
        public RepeatingType RepeatingType
        {
            get; set;
        }
    }
}
