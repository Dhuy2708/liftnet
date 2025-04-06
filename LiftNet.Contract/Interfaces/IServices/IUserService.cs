using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using LiftNet.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IServices
{
    public interface IUserService : IDependency
    {
        Task<Dictionary<string, LiftNetRoleEnum>> GetUserIdRoleDict(List<string> userIds);
        Task<List<User>> GetByIdsAsync(List<string> userIds);
        Task<BasicUserInfo?> GetBasicUserInfoAsync(string userId);
    }
}
