using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
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
                                       .Select(x => new { x.Id, x.UserRoles })
                                       .ToListAsync();

            foreach (var user in users)
            {
                if (roleDict.TryGetValue(user.UserRoles.FirstOrDefault()?.RoleId!, out var roleEnum))
                {
                    result.Add(user.Id, roleEnum);
                }
            }
            return result;
        }

        public async Task<List<User>> GetByIdsAsync(List<string> userIds)
        {
            return (await _userRepo.GetByIds(userIds)).ToList();
        }
    }
}
