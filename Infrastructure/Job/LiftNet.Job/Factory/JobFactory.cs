﻿using LiftNet.Contract.Enums.Job;
using LiftNet.Contract.Interfaces.IJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Threading.Tasks;

namespace LiftNet.Job.Factory
{
    public class JobFactory
    {
        private readonly Dictionary<JobType, Lazy<IActionJob>> _lazyJobActions = [];
        private readonly Dictionary<JobType, Lazy<ISystemJob>> _lazyMainJobs = [];

        public JobFactory(IServiceProvider provider)
        {
            // main

            // action
            // 
        }

        public IActionJob GetActionJob(JobType jobType)
        {
            if (_lazyJobActions.TryGetValue(jobType, out var lazyJobAction))
            {
                return lazyJobAction.Value;
            }
            throw new NotSupportedException($"Action job Type {jobType} not supported.");
        }

        public ISystemJob GetSystemJob(JobType jobType)
        {
            if (_lazyMainJobs.TryGetValue(jobType, out var lazyJobDisc))
            {
                return lazyJobDisc.Value;
            }
            throw new NotSupportedException($"Main job Type {jobType} not supported.");
        }
    }
}
