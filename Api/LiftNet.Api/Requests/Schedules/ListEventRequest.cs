namespace LiftNet.Api.Requests.Schedules
{
    public class ListEventRequest
    {
        public DateOnly StartDate
        {
            get; set;
        }

        public DateOnly EndDate
        {
            get; set;
        }

        public string? EventSearch
        {
            get; set;
        }

        public List<string>? UserIds
        {
            get; set;
        }
    }
}
