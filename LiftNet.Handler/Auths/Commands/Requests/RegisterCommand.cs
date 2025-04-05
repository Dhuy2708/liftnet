using LiftNet.Contract.Dtos;
using LiftNet.Domain.Enums;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Auths.Commands.Requests
{
    public class RegisterCommand : IRequest<LiftNetRes>
    {
        public LiftNetRoleEnum Role
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
        public string Password
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
        public string PlaceId
        {
            get; set;
        }
    }
}
