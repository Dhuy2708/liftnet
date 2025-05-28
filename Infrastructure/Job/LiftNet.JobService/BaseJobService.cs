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
        protected readonly JobType _jobType;
        protected readonly IServiceProvider _provider;

        public BaseJobService(JobType jobType, IServiceProvider provider)
        {
            _jobType = jobType;
            _provider = provider;
        }

        public abstract Task KickOffAsync();
    }
}
