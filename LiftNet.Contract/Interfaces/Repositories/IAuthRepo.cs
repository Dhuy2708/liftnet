using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.Repositories
{
    public interface IAuthRepo
    {
        Task<string> LogInAsync(LoginRequest logInViewModel);
        Task<IdentityResult> RegisterAsync(RegisterRequest registerViewModel);
    }
}
