using LiftNet.Contract.Enums.Finder;

namespace LiftNet.Api.Requests.Finders
{
    public class ResponseApplicantReq
    {
        public int ApplicantId
        {
            get; set;
        }
        public FinderPostResponseType Status
        {
            get; set;
        }
    }
}
