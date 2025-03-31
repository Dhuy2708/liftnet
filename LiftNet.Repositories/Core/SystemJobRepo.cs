using LiftNet.Contract.Enums.Job;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Persistence.Context;
using LiftNet.Utility.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Repositories.Core
{
    public class SystemJobRepo : ISystemJobRepo
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private LiftNetDbContext _dbContext;
        private ILiftLogger<SystemJobRepo> _logger;


        public SystemJobRepo(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<SystemJob?> GetLastJob(JobType type)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILiftLogger<SystemJobRepo>>();
                if (!SystemJobFamily.Contains(type))
                {
                    _logger.Error($"Job type {type} is not a system job");
                    return null;
                }

                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LiftNetDbContext>();
                    return await dbContext.SystemJobs
                                          .Where(x => x.Type == (int)type)
                                          .OrderByDescending(x => x.StartTime)
                                          .FirstOrDefaultAsync();
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error system getting job");
                    return null;
                }
            }
        }

        public async Task<SystemJob?> InsertJob(JobType type)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILiftLogger<SystemJobRepo>>();

                if (!SystemJobFamily.Contains(type))
                {
                    logger.Error($"job type {type} is not system job");
                }

                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LiftNetDbContext>();

                    var systemJob = await GetLastJob(type);
                    if (systemJob!.EndTime == null || systemJob.Status != (int)JobStatus.Finished)
                    {
                        logger.Error("last job not finished, return");
                        return null;
                    }
                    
                    var inserted = new SystemJob()
                    {
                        Id = JobUtil.GetJobId(type),
                        Type = (int)type,
                        StartTime = DateTime.UtcNow,
                        EndTime = null,
                        Status = (int)JobStatus.Waiting,
                    };
                    await dbContext.SystemJobs.AddAsync(inserted);
                    return inserted;
                }
                catch (Exception e)
                {
                    logger.Error($"error while insert job, job type: {type}, ex: {e.Message}");
                    return null;
                }
            }
        }

        public async Task<int> UpdateJobStatus(string jobId, JobStatus status)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILiftLogger<SystemJobRepo>>();
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LiftNetDbContext>();

                    var job = dbContext.SystemJobs.Where(x => x.Id == jobId).FirstOrDefault();
                    if (job == null)
                    {
                        logger.Error($"job with jobId: {jobId} doesnt exist");
                        return 0;
                    }
                    job.Status = (int)status;
                    await dbContext.SaveChangesAsync();
                    return 1;
                }
                catch (Exception e)
                {
                    logger.Error($"error while update job status, job id: {jobId}, ex: {e.Message}");
                    return 0;
                }
            }
        }
    }
}
