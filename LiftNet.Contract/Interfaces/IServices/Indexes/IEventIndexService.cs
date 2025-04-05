using LiftNet.Domain.Indexes;
using LiftNet.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IServices.Indexes
{
    public interface IEventIndexService : IIndexBaseService<EventIndexData>, IDependency
    {
    }
}
