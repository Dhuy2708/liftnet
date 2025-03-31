using LiftNet.Contract.Enums.Job;
using LiftNet.Contract.Interfaces.IJobs;
using LiftNet.Contract.Interfaces.IRepos;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.JobService.System
{
    public abstract class BaseSystemJobService : BaseJobService, ISystemJob
    {
        private readonly ISystemJobRepo _systemJobRepo;
        protected BaseSystemJobService(JobType jobType, IServiceProvider provider) : base(jobType, provider)
        {
            _systemJobRepo = provider.GetRequiredService<ISystemJobRepo>();
        }

        protected async Task<bool> CheckJobCanRun(TimeSpan intervalTime)
        {
            var job = await _systemJobRepo.GetLastJob(_jobType);
            if (job == null)
            {
                return true;
            }
            if (job.EndTime == null || job.Status != (int)JobStatus.Finished)
            {
                return false;
            }
            return DateTime.UtcNow.Subtract(job.EndTime.Value) > intervalTime;
        }
    }
}
