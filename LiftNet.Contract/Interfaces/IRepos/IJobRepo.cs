using LiftNet.Contract.Enums.Job;
using LiftNet.Domain.Entities;
using LiftNet.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IRepos
{
    public interface IJobRepo
    {
        Task<int> InsertJob(JobType type, JobStatus status, string userId = null);
        Task<int> UpdateJobFailed(JobType type);
        Task<int> UpdateJobFinished(JobType type);
        Task<SystemJob?> GetSystemJob(JobType type);
        Task<CustomerJob?> GetCustomerJob(JobType type);
        Task<ActionJob?> GetActionJob(JobType type);
    }
}
