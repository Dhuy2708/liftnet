namespace LiftNet.Api.Requests.Appointments
{
    public class AppointmentFeedbackReq
    {
        public string AppointmentId
        {
            get; set;
        }

        public List<IFormFile>? Medias
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
