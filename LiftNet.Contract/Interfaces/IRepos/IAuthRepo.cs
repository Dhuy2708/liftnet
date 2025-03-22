using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiftNet.Contract.Dtos.Auth;
using System.IdentityModel.Tokens.Jwt;

namespace LiftNet.Contract.Interfaces.IRepos
{
    public interface IAuthRepo
    {
        Task<JwtSecurityToken?> LogInAsync(LoginModel logInViewModel);
        Task<IdentityResult> RegisterAsync(RegisterModel registerViewModel);
        Task LogOutAsync();
    }
}
