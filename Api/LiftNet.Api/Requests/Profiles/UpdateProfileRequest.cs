using LiftNet.Contract.Enums;
using LiftNet.Domain.Enums;

namespace LiftNet.Api.Requests.Profiles
{
    public class UpdateProfileRequest
    {
        public string FirstName
        {
            get; set;
        }
        public string LastName
        {
            get; set;
        }
        public string Bio
        {
            get; set;
        }
        public string Location
        {
            get; set;
        }
        public LiftNetGender Gender
        {
            get; set;
        }
        public int Age
        {
            get; set;
        }
    }
}
