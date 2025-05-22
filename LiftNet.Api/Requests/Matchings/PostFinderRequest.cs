using LiftNet.Api.Requests.Auths;
using LiftNet.Contract.Enums;
using LiftNet.Contract.Enums.Appointment;

namespace LiftNet.Api.Requests.Matchings
{
    public class PostFinderRequest
    {
        public string Title
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public DateTime StartTime
        {
            get; set;
        }

        public DateTime EndTime
        {
            get; set;
        }

        public int StartPrice
        {
            get; set;
        }

        public int EndPrice
        {
            get; set;
        }

        public string LocationId
        {
            get; set;
        }

        public bool HideAddress
        {
            get; set;
        }

        public RepeatingType RepeatType
        {
            get; set;
        }
    }
}
