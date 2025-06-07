using LiftNet.Contract.Dtos;
using LiftNet.Domain.Enums;
using LiftNet.Domain.Response;
using MediatR;

namespace LiftNet.Handler.Admins.Commands.Requests
{
    public class CreateUserCommand : IRequest<LiftNetRes>
    {
        public string CallerId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Age { get; set; }
        public LiftNetGender? Gender { get; set; }
        public LiftNetRoleEnum Role { get; set; }
    }
} 