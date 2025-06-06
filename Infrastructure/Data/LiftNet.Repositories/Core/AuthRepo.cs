using LiftNet.Contract.Constants;
using LiftNet.Contract.Dtos.Auth;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Constants;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using LiftNet.Domain.Interfaces;
using LiftNet.Ioc;
using LiftNet.Utility.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LiftNet.Repositories.Core
{
    public class AuthRepo : IAuthRepo
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ILiftLogger<AuthRepo> _logger;

        public AuthRepo(UserManager<User> userManager,
                        SignInManager<User> signInManager,
                        ILiftLogger<AuthRepo> logger,
                        RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterModel model)
        {
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Username,
                CreatedAt = DateTime.UtcNow,
                ProvinceCode = model.ProvinceCode,
                DistrictCode = model.DistrictCode,
                WardCode = model.WardCode,
                AddressId = model.AddressId,
                Age = model.Age,
                Gender = (int)model.Gender,
                IsDeleted = false,
                IsSuspended = false,
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(model.Role.ToString()))
                {
                    await _roleManager.CreateAsync(new Role(model.Role.ToString()));
                }
                await _userManager.AddToRoleAsync(user, model.Role.ToString());
            }
            return result;
        }

        public async Task<JwtSecurityToken?> LogInAsync(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null || user.IsDeleted == true || user.IsSuspended == true)
            {
                return null;
            }

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
            if (!result.Succeeded)
            {
                _logger.Error(result.ToString());
                return null;
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(LiftNetClaimType.UId, user.Id),
                new Claim(LiftNetClaimType.UEmail, user.Email ?? ""),
                new Claim(LiftNetClaimType.Username, user.UserName ?? ""),
                new Claim(LiftNetClaimType.FirstName, user.FirstName),
                new Claim(LiftNetClaimType.LastName, user.LastName),
                new Claim(LiftNetClaimType.UAvatar, user.Avatar),
                new Claim(LiftNetClaimType.Roles, string.Join(", ", userRoles.ToList())),
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
                expires: DateTime.UtcNow.AddSeconds(CoreConstant.TokenExpirationTimeInSeconds),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
            );
            return token;
        }

        public async Task<JwtSecurityToken?> AdminLoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null || user.IsDeleted == true || user.IsSuspended == true)
            {
                return null;
            }

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
            if (!result.Succeeded)
            {
                _logger.Error(result.ToString());
                return null;
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Contains(LiftNetRoles.Admin))
            {
                _logger.Error("User is not an admin");
                return null;
            }
            var authClaims = new List<Claim>
            {
                new Claim(LiftNetClaimType.UId, user.Id),
                new Claim(LiftNetClaimType.UEmail, user.Email ?? ""),
                new Claim(LiftNetClaimType.Username, user.UserName ?? ""),
                new Claim(LiftNetClaimType.FirstName, user.FirstName),
                new Claim(LiftNetClaimType.LastName, user.LastName),
                new Claim(LiftNetClaimType.UAvatar, user.Avatar),
                new Claim(LiftNetClaimType.Roles, string.Join(", ", userRoles.ToList())),
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
                expires: DateTime.UtcNow.AddSeconds(CoreConstant.TokenExpirationTimeInSeconds),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
            );
            return token;
        }

        public async Task LogOutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
