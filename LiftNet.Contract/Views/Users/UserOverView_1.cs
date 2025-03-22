using LiftNet.Contract.Dtos;

namespace LiftNet.Contract.Views.Users
{
    public class UserOverView
    {
        public string Id
        {
            get; set;
        }
        public string Email
        {
            get; set;
        }
        public string Username
        {
            get; set;
        }
        public string FirstName
        {
            get; set;
        }
        public string LastName
        {
            get; set;
        }
        public string Avatar
        {
            get; set;
        }
        public bool IsDeleted
        {
            get; set;
        }
        public bool IsSuspended
        {
            get; set;
        }
    }
}
