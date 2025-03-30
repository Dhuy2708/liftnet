using LiftNet.Contract.Enums.Job;
using LiftNet.Contract.Interfaces.IJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.JobService.System
{
    public abstract class BaseSystemJobService : BaseJobService, ISystemJob
    {
        protected BaseSystemJobService(JobType jobType) : base(jobType)
        {
        }
    }
}
