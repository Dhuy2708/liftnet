using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Enums.Job
{
    public enum JobType
    {
        None = 0,
        ProvinceDiscovery = 1,
    }

    public enum JobStatus
    {
        None = 0,
        Waiting = 1,
        InProgress = 2,
        Finished = 3,
        Failed = 4,
    }
}
