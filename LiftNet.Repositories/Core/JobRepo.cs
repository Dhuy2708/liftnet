using LiftNet.Contract.Enums.Job;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Persistence.Context;
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

        public Task<SystemJob> GetJob(JobType type)
        {
            throw new NotImplementedException();
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<LiftNetDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILiftLogger<JobRepo>>();


            }
        }

        public Task<int> InsertJob(JobType type, string userId = null)
        {
            throw new NotImplementedException();
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
