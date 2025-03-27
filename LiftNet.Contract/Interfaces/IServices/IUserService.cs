using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IServices
{
    public interface IUserService
    {
        Task<Dictionary<string, LiftNetRoleEnum>> GetUserIdRoleDict(List<string> userIds);
        Task<List<User>> GetByIdsAsync(List<string> userIds);
    }
}
