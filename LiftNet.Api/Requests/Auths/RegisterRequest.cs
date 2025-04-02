using LiftNet.Contract.Dtos;
using LiftNet.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace LiftNet.Api.Requests.Auths
{
    public class RegisterRequest
    {
        public LiftNetRoleEnum Role
        {
            get; set;
        }
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
        public AddressRequest? Address
        {
            get; set;
        }
    }

    public class AddressRequest
    {
        public int ProvinceCode
        {
            get; set; 
        }
        public int DistrictCode
        {
            get; set;
        }
        public int WardCode
        {
            get; set;
        }
        public string Location
        {
            get; set;
        }
    }
}
