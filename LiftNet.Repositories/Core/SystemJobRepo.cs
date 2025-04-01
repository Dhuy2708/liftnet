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
        private LiftNetDbContext _dbContext;
        private ILiftLogger<SystemJobRepo> _logger;

        public SystemJobRepo(LiftNetDbContext dbContext, ILiftLogger<SystemJobRepo> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<SystemJob?> GetLastJob(JobType type)
        {
            if (!SystemJobFamily.Contains(type))
            {
                _logger.Error($"Job type {type} is not a system job");
                return null;
            }

            try
            {
                return await _dbContext.SystemJobs
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

        public async Task<SystemJob?> InsertJob(JobType type)
        {
            if (!SystemJobFamily.Contains(type))
            {
                _logger.Error($"job type {type} is not system job");
            }

            try
            {
                var systemJob = await GetLastJob(type);
                if (systemJob == null)
                {
                    var firstInserted = new SystemJob()
                    {
                        Id = JobUtil.GetJobId(type),
                        Type = (int)type,
                        StartTime = DateTime.UtcNow,
                        EndTime = null,
                        Status = (int)JobStatus.Waiting,
                    };
                    await _dbContext.SystemJobs.AddAsync(firstInserted);
                    await _dbContext.SaveChangesAsync();
                    return firstInserted;
                }

                if (systemJob!.EndTime == null || systemJob.Status != (int)JobStatus.Finished)
                {
                    _logger.Error("last job not finished, return");
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
                await _dbContext.SystemJobs.AddAsync(inserted);
                await _dbContext.SaveChangesAsync();
                return inserted;
            }
            catch (Exception e)
            {
                _logger.Error($"error while insert job, job type: {type}, ex: {e.Message}");
                return null;
            }
        }

        public async Task<int> UpdateJobStatus(string jobId, JobStatus status)
        {
            try
            {
                var job = _dbContext.SystemJobs.Where(x => x.Id == jobId).FirstOrDefault();
                if (job == null)
                {
                    _logger.Error($"job with jobId: {jobId} doesnt exist");
                    return 0;
                }
                job.Status = (int)status;
                await _dbContext.SaveChangesAsync();
                return 1;
            }
            catch (Exception e)
            {
                _logger.Error($"error while update job status, job id: {jobId}, ex: {e.Message}");
                return 0;
            }
        }
    }
}
