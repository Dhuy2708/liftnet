using LiftNet.Domain.Enums;
using LiftNet.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IServices
{
    public interface IRoleService : IDependency
    {
        Task<LiftNetRoleEnum> GetRoleByUserId(string userId);
    }
}
