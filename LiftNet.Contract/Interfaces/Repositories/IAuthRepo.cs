using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiftNet.Contract.Dtos.Auth;

namespace LiftNet.Contract.Interfaces.Repositories
{
    public interface IAuthRepo
    {
        Task<string> LogInAsync(LoginModel logInViewModel);
        Task<IdentityResult> RegisterAsync(RegisterModel registerViewModel);
        Task LogOutAsync();
    }
}
