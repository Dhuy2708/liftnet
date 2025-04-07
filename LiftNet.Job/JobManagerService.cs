using LiftNet.Contract.Enums.Job;
using LiftNet.Job.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Job
{
    public class JobManagerService
    {
        private readonly IServiceProvider _provider;
        private readonly JobFactory _jobFactory;
        public JobManagerService(IServiceProvider provider)
        {
            _provider = provider;
            _jobFactory = new JobFactory(provider);
        }

        public async Task ExecuteJobAsync(JobType jobType)
        {
            var job = _jobFactory.GetActionJob(jobType);


            await job.KickOffAsync();
        }
    }
}
