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
    public class JobRepo : IJobRepo
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private LiftNetDbContext _dbContext;
        private ILiftLogger<JobRepo> _logger;


        public JobRepo(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<ActionJob?> GetActionJob(JobType type)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILiftLogger<JobRepo>>();
                if (!ActionJobFamily.Contains(type))
                {
                    _logger.Error($"Job type {type} is not an action job");
                    return null;
                }

                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LiftNetDbContext>();
                    return await dbContext.ActionJobs.FirstOrDefaultAsync(x => x.Type == (int)type);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error getting action job");
                    return null;
                }
            }
        }

        public async Task<CustomerJob?> GetCustomerJob(JobType type)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILiftLogger<JobRepo>>();

                if (!CustomerJobFamily.Contains(type))
                {
                    _logger.Error($"Job type {type} is not a customer job");
                    return null;
                }

                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LiftNetDbContext>();
                    return await dbContext.CustomerJobs.FirstOrDefaultAsync(x => x.Type == (int)type);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error getting customer job");
                    return null;
                }
            }
        }

        public async Task<SystemJob?> GetSystemJob(JobType type)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILiftLogger<JobRepo>>();
                if (!SystemJobFamily.Contains(type))
                {
                    _logger.Error($"Job type {type} is not a system job");
                    return null;
                }

                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LiftNetDbContext>();

                    return await dbContext.SystemJobs.FirstOrDefaultAsync(x => x.Type == (int)type);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error system getting job");
                    return null;
                }
            }
        }

        public async Task<int> InsertJob(JobType type, string userId = null)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILiftLogger<JobRepo>>();

                var kind = JobKind.None;
                if (SystemJobFamily.Contains(type))
                {
                    kind = JobKind.System;
                }
                else if (ActionJobFamily.Contains(type))
                {
                    kind = JobKind.Action;
                }
                else if (CustomerJobFamily.Contains(type))
                {
                    kind = JobKind.Customer;
                }

                if (kind is JobKind.None)
                {
                    _logger.Error($"Cant find job kind of job type {type}");
                    return 0;
                }

                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LiftNetDbContext>();

                    switch (kind)
                    {
                        case JobKind.System:
                            var systemJob = await dbContext.SystemJobs.FirstOrDefaultAsync(x => x.Type == (int)type);
                            if (systemJob == null)
                            {
                                await dbContext.SystemJobs.AddAsync(new SystemJob()
                                {
                                    Id = JobUtil.GetJobId(type),
                                    Type = (int)type,
                                    StartTime = DateTime.UtcNow,
                                    EndTime = null,
                                    Status = (int)JobStatus.Waiting,
                                });
                                return 1;
                            }
                            systemJob.Status
                    }
                }
            }
        }

        public Task<int> UpdateJobFailed(JobType type)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateJobFinished(JobType type)
        {
            throw new NotImplementedException();
        }
    }
}
