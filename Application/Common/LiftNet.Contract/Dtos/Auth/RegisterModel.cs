using LiftNet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Dtos.Auth
{
    public class RegisterModel
    {
        public required LiftNetRoleEnum Role
        {
            get; set;
        }
        public required string FirstName
        {
            get; set;
        }
        public required string LastName
        {
            get; set;
        }
        public required string Email
        {
            get; set;
        }
        public required string Username
        {
            get; set;
        }
        public required string Password
        {
            get; set;
        }
        public int Age { get; set; }
        public LiftNetGender Gender { get; set; }
        public int? ProvinceCode
        {
            get; set;
        }
        public int? DistrictCode
        {
            get; set;
        }
        public int? WardCode
        {
            get; set;
        }
        public string? AddressId
        {
            get; set;
        }
    }
}
