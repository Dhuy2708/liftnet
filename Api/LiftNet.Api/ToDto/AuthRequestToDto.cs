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
                Role = request.Role,
                Email = request.Email,
                Username = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Address = request.Address != null ? new Handler.Auths.Commands.Requests.AddressRequest()
                {
                    ProvinceCode = request.Address!.ProvinceCode,
                    DistrictCode = request.Address!.DistrictCode,
                    WardCode = request.Address!.WardCode,
                    Location = request.Address!.Location,
                } : null,
                Age = request.Age ?? 0,
                Gender = request.Gender ?? 0,
            };
        }

        public static LoginCommand ToCommand(this LoginRequest request)
        {
            return new LoginCommand
            {
                Username = request.Email,
                Password = request.Password
            };
        }
    }
}
