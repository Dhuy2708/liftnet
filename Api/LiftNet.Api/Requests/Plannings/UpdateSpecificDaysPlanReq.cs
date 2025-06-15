namespace LiftNet.Api.Requests.Plannings
{
    public class UpdateSpecificDaysPlanReq
    {
        public string UserId { get; set; }
        public List<int> DaysOfWeek { get; set; }
        public List<ExerciseReq> Plan { get; set; }
    }
} 