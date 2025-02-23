using LiftNet.Contract.Dtos;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Auth.Commands.Requests
{
    public class RegisterCommand : IRequest<LiftNetRes>
    {
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
        public AddressDto? Address
        {
            get; set;
        }
    }
}
