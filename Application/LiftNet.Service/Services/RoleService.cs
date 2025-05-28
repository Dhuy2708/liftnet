using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using LiftNet.Domain.Interfaces;
using LiftNet.Ioc;
using LiftNet.Utility.Extensions;
using LiftNet.Utility.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LiftNet.Service.Services
{
    internal class RoleService : IRoleService
    {
        private readonly ILiftLogger<RoleService> _logger;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUnitOfWork _uow;

        public RoleService(ILiftLogger<RoleService> logger, RoleManager<Role> roleManager, IUnitOfWork uow)
        {
            _logger = logger;
            _roleManager = roleManager;
            _uow = uow;
        }

        public async Task<LiftNetRoleEnum> GetRoleByUserId(string userId)
        {
            try
            {
                var roleId = await _uow.UserRepo.GetQueryable()
                                        .Where(x => x.Id == userId)
                                        .Select(x => x.UserRoles.First().RoleId)
                                        .FirstOrDefaultAsync();
                if (roleId == null)
                {
                    _logger.Error($"cant get role of user, userId: {userId}");
                    return LiftNetRoleEnum.None;
                }
                var roles = await _roleManager.Roles.ToListAsync();
                var roleName = roles.FirstOrDefault(x => x.Id == roleId)?.Name;
                if (roleName.IsNotNullOrEmpty())
                {
                    return RoleUtil.GetRole(roleName!);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get role by user id");
            }
            return LiftNetRoleEnum.None;
        }

        public async Task<Dictionary<string, LiftNetRoleEnum>> GetAllRoleDictAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles.ToDictionary(x => x.Id, x => RoleUtil.GetRole(x.Name!));
        }
    }
}
