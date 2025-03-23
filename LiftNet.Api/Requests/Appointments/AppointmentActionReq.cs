using LiftNet.Contract.Enums;

namespace LiftNet.Api.Requests.Appointments
{
    public class AppointmentActionReq
    {
        public string AppointmentId
        {
            get; set;
        }

        public AppointmentActionRequestType Action
        {
            get; set;
        }
    }
}
