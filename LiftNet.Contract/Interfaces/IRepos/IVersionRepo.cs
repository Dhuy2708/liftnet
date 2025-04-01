using LiftNet.Domain.Entities;
using LiftNet.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiftNetVersion = LiftNet.Domain.Entities.LiftNetVersion;

namespace LiftNet.Contract.Interfaces.IRepos
{
    public interface IVersionRepo : ICrudBaseRepo<LiftNetVersion>, IDependency
    {
    }
}
