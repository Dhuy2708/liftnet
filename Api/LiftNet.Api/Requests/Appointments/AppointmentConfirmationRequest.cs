namespace LiftNet.Api.Requests.Appointments
{
    public class AppointmentConfirmationRequest
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
    }
}
