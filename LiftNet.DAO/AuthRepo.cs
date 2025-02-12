using LiftNet.Contract.Interfaces.Repositories;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LiftNet.Repositories
{
    internal class AuthRepo : IAuthRepo
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILiftLogger<AuthRepo> _logger;

        public AuthRepo(UserManager<User> userManager,
                        SignInManager<User> signInManager,
                        ILiftLogger<AuthRepo> logger,
                        RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterRequest model)
        {
            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                UserName = model.Username,
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                IsSuspended = false,
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(RoleEnum.Student.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole(RoleEnum.Student.ToString()));
                }
                StudentDto student = new StudentDto
                {
                    User = user,
                };
                await _studentService.CreateAsync(student);
                await _userManager.AddToRoleAsync(user, RoleEnum.Student.ToString());
            }
            return result;
        }

        public async Task<string> LogInAsync(LoginRequest model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null || user.IsDeleted == true || user.IsSuspended == true)
            {
                return string.Empty;
            }

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
            if (!result.Succeeded)
            {
                _logger.LogError(result.ToString());
                return string.Empty;
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.NameIdentifier, user.UserName ?? ""),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, string.Join(", ", userRoles.ToList())),
                new Claim("avatar", user.Avatar),

                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var util = CoreUtil.Instance();
            var authenKey = new SymmetricSecurityKey(util.GetSecretKey());
            var issuer = util.GetIssuer();
            var audience = util.GetValidAudience();

            var token = new JwtSecurityToken
            (
                issuer: issuer,
                audience: audience,
                expires: DateTime.Now.AddSeconds(CoreConstants.TokenExpirationTimeInSeconds),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
