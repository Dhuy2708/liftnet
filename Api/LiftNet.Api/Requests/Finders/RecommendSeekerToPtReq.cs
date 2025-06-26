namespace LiftNet.Api.Requests.Finders
{
    public class RecommendSeekerToPtReq
    {
        public string SeekerId
        {
            get; set;
        }

        public List<string> PTIds
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }
    }
}
