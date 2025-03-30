using LiftNet.Contract.Enums.Job;
using LiftNet.Contract.Interfaces.IJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.JobService
{
    public abstract class BaseJobService : IJob
    {
        private readonly JobType _jobType;

        public BaseJobService(JobType jobType)
        {
            _jobType = jobType;
        }

        public abstract Task KickOffAsync();
    }
}
