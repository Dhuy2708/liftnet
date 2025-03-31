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
    public interface ISystemJobRepo
    {
        Task<SystemJob?> InsertJob(JobType type);
        Task<int> UpdateJobStatus(string jobId, JobStatus status);
        Task<SystemJob?> GetLastJob(JobType type);
    }
}
