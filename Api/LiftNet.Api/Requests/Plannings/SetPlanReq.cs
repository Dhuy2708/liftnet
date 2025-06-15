namespace LiftNet.Api.Requests.Plannings
{
    public class SetPlanReq
    {
        public string UserId
        {
            get; set;
        }
        public List<ExerciseReq> Plan
        {
            get; set;
        }
    }

    public class ExerciseReq
    {
        public int DayOfWeek
        {
            get; set;
        }
        public List<string> ExerciseIds
        {
            get; set;
        }
    }
}
