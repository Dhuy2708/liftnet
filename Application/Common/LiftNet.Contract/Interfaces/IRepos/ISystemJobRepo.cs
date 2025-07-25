﻿using LiftNet.Contract.Enums.Job;
using LiftNet.Domain.Entities;
using LiftNet.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IRepos
{
    public interface ISystemJobRepo : IDependency
    {
        Task<SystemJob?> GetLastJob(JobType type);
        Task<SystemJob?> InsertJob(JobType type, bool overwrite = true);
        Task<int> UpdateJobStatus(string jobId, JobStatus status);
        Task<int> FinishJob(string jobId, bool isSuccess, DateTime? scanTime = null);
    }
}
