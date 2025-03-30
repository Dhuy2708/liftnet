using LiftNet.Contract.Enums.Job;
using LiftNet.Contract.Interfaces.IJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.JobService.System
{
    public class ProvinceDiscoveryService : BaseSystemJobService
    {
        private TimeSpan IntervalTime = TimeSpan.FromHours(1);
        public ProvinceDiscoveryService(JobType jobType) : base(jobType)
        {
        }

        public override Task KickOffAsync()
        {
            try
            {

            }
        }
    }
}
