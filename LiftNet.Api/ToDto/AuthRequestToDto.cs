using LiftNet.Api.Requests.Auths;
using LiftNet.Handler.Auths.Commands.Requests;

namespace LiftNet.Api.ToDto
{
    public static class AuthRequestToDto
    {
        public static RegisterCommand ToCommand(this RegisterRequest request)
        {
            return new RegisterCommand
            {
                Email = request.Email,
                Username = request.Username,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Address = request.Address
            };
        }

        public static LoginCommand ToCommand(this LoginRequest request)
        {
            return new LoginCommand
            {
                Username = request.Username,
                Password = request.Password
            };
        }
    }
}
