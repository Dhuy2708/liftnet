using LiftNet.Contract.Constants;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.CosmosDb.Contracts;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.CosmosDb.Services
{
    public class EventIndexService : IndexBaseService<EventIndexData>, IEventIndexService
    {
        public EventIndexService(CosmosCredential cred) 
                : base(cred, CosmosContainerId.Schedule)
        {
        }
    }
}
