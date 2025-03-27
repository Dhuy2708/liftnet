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
    public class ScheduleIndexService : IndexBaseService<ScheduleIndexData>, IScheduleIndexService
    {
        public ScheduleIndexService(CosmosCredential cred, ILiftLogger<IndexBaseService<ScheduleIndexData>> logger) : base(cred, CosmosContainerId.Schedule, logger)
        {
        }
    }
}
