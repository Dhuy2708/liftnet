namespace LiftNet.Api.Requests.Plannings
{
    public class RemoveExerciseFromPlanReq
    {
        public string UserId { get; set; }
        public int DayOfWeek { get; set; }
        public int Order { get; set; }
    }
} 