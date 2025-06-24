namespace LiftNet.Api.Requests.Plannings
{
    public class AddExerciseToPlanReq
    {
        public string UserId { get; set; }
        public int DayOfWeek { get; set; }
        public string ExerciseId { get; set; }
    }
} 