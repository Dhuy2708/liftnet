using LiftNet.Contract.Dtos;

namespace LiftNet.Contract.Views.Users
{
    public class UserView
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
        public AddressView Address
        {
            get; set;
        }
        public DateTime CreatedAt
        {
            get; set;
        } = DateTime.UtcNow;
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
