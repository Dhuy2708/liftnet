using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiftNet.Contract.Dtos.Auth;
using System.IdentityModel.Tokens.Jwt;
using LiftNet.Ioc;

namespace LiftNet.Contract.Interfaces.IRepos
{
    public interface IAuthRepo : IDependency
    {
        Task<JwtSecurityToken?> LogInAsync(LoginModel model);
        Task<JwtSecurityToken?> AdminLoginAsync(LoginModel model);
        Task<IdentityResult> RegisterAsync(RegisterModel registerViewModel);
        Task LogOutAsync();
    }
}
