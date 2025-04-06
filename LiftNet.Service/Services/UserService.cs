using LiftNet.Contract.Constants;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Domain.Constants;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using LiftNet.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Service.Services
{
    public class UserService : IUserService
    {
        private readonly ILiftLogger<UserService> _logger;
        private readonly IUserRepo _userRepo;
        private readonly IRoleService _roleService;

        public UserService(ILiftLogger<UserService> logger, IUserRepo userRepo, IRoleService roleService)
        {
            _logger = logger;
            _userRepo = userRepo;
            _roleService = roleService;
        }

        public async Task<Dictionary<string, LiftNetRoleEnum>> GetUserIdRoleDict(List<string> userIds)
        {
            var result = new Dictionary<string, LiftNetRoleEnum>();
            var roleDict = await _roleService.GetAllRoleDictAsync();
            var users = await _userRepo.GetQueryable()
                                     .Include(x => x.UserRoles)
                                     .Where(x => userIds.Contains(x.Id))
                                     .ToListAsync();

            foreach (var user in users)
            {
                var userRole = user.UserRoles.FirstOrDefault();
                if (userRole != null && roleDict.TryGetValue(userRole.RoleId, out var roleEnum))
                {
                    result[user.Id] = roleEnum;
                }
                else
                {
                    result[user.Id] = LiftNetRoleEnum.None;
                }
            }
            return result;
        }

        public async Task<List<User>> GetByIdsAsync(List<string> userIds)
        {
            return await _userRepo.GetQueryable()
                .Include(x => x.UserRoles)
                .Where(x => userIds.Contains(x.Id))
                .ToListAsync();
        }

        public async Task<BasicUserInfo?> GetBasicUserInfoAsync(string userId)
        {
            try
            {
                var user = await _userRepo.GetQueryable()
                    .Include(x => x.UserRoles)
                    .FirstOrDefaultAsync(x => x.Id == userId && !x.IsDeleted && !x.IsSuspended);

                if (user == null)
                {
                    _logger.Warn($"User not found or inactive with ID: {userId}");
                    return null;
                }

                var roleDict = await _roleService.GetAllRoleDictAsync();
                var userRole = user.UserRoles.FirstOrDefault();
                var role = LiftNetRoleEnum.None;

                if (userRole != null && roleDict.TryGetValue(userRole.RoleId, out var roleEnum))
                {
                    role = roleEnum;
                }

                var basicInfo = new BasicUserInfo
                {
                    Id = user.Id,
                    Username = user.UserName ?? "",
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Avatar = user.Avatar ?? CoreConstant.DEFAULT_USER_AVATAR,
                    Role = role
                };

                _logger.Info($"Successfully retrieved basic info for user: {userId}");
                return basicInfo;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting basic info for user: {userId}");
                return null;
            }
        }
    }
}
