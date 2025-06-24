namespace LiftNet.Api.Requests.Plannings
{
    public class ReorderExerciseReq
    {
        public int SourceDayOfWeek { get; set; }
        public int SourceOrder { get; set; }
        public int TargetDayOfWeek { get; set; }
        public int TargetOrder { get; set; }
        public string ExerciseId { get; set; }
    }
} 