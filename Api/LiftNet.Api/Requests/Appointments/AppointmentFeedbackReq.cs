namespace LiftNet.Api.Requests.Appointments
{
    public class AppointmentFeedbackReq
    {
        public string AppointmentId
        {
            get; set;
        }

        public IFormFile? Image
        {
            get; set;
        }

        public string? Content
        {
            get; set;
        }

        public int Star
        {
            get; set;
        }
    }
}
