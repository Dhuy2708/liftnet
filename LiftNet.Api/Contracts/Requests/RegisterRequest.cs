using LiftNet.Contract.Dtos;
using System.ComponentModel.DataAnnotations;

namespace LiftNet.Api.Contracts.Requests
{
    public class RegisterRequest
    {
        public string Email
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
        public string Username
        {
            get; set;
        }
        public string Password
        {
            get; set;
        }
        public AddressDto? Address
        {
            get; set;
        }
    }
}
