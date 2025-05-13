using LiftNet.Contract.Enums.Job;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl.AdoJobStore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Timer.Service.Common
{
    public abstract class BaseSystemJob : BaseJob
    {
        protected ISystemJobRepo _systemJobRepo => _provider.GetRequiredService<ISystemJobRepo>();
        private ILiftLogger<BaseSystemJob> _logger => _provider.GetRequiredService<ILiftLogger<BaseSystemJob>>();

        protected readonly JobType _jobType;
        protected readonly TimeSpan _intervalTime;
        protected DateTime _scanTime => DateTime.UtcNow;

        protected BaseSystemJob(JobType jobType, IServiceProvider provider, TimeSpan? intervalTime = null) : base(provider)
        {
            _jobType = jobType;
            _intervalTime = intervalTime ?? TimeSpan.FromMinutes(10);
        }

        // this should be for main job, timer job already config interval time 
        //protected async Task<bool> CheckJobCanRun(TimeSpan intervalTime)
        //{
        //    var job = await _systemJobRepo.GetLastJob(_jobType);
        //    if (job == null)
        //    {
        //        return true;
        //    }
        //    if (job.EndTime == null || !(job.Status == (int)JobStatus.Finished || job.Status == (int)JobStatus.Failed))
        //    {
        //        return false;
        //    }
        //    return _scanTime.Subtract(job.EndTime.Value) > intervalTime;
        //}

        protected async Task<bool> CheckJobCanRun()
        {
            var job = await _systemJobRepo.GetLastJob(_jobType);
            if (job == null)
            {
                return true;
            }
            if (job.EndTime == null || !(job.Status == (int)JobStatus.Finished || job.Status == (int)JobStatus.Failed))
            {
                return false;
            }
            if (job.Status == (int)JobStatus.Finished)
            {
                return _scanTime.Subtract(job.EndTime.Value) > _intervalTime;
            }
        
            return _scanTime.Subtract(job.StartTime) > _intervalTime;
        }

        public override async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.Info($"begin job, type: {_jobType}");
#if !DEBUG
                if (!await CheckJobCanRun())
                {
                    _logger.Info($"job has been run before, skip");
                    return;
                }
#endif

                var job = await _systemJobRepo.InsertJob(_jobType);

                if (job == null)
                {
                    _logger.Error($"cant insert job, job type: {_jobType}");
                    return;
                }

                try
                {
                    _ = await _systemJobRepo.UpdateJobStatus(job.Id, JobStatus.InProgress);
                    var status = await KickOffJobServiceAsync();
                    if (status is JobStatus.Finished)
                    {
                        _ = await _systemJobRepo.FinishJob(job.Id, true, _scanTime);
                    }
                    else
                    {
                        _ = await _systemJobRepo.FinishJob(job.Id, false, _scanTime);
                    }
                }
                catch (Exception jobEx)
                {
                    _ = await _systemJobRepo.UpdateJobStatus(job.Id, JobStatus.Failed);
                    _logger.Error(jobEx, $"an error occured during the job, type: {_jobType}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"an error occured while operate the job, type: {_jobType}");
            }
        }

        protected abstract Task<JobStatus> KickOffJobServiceAsync();
    }
}
