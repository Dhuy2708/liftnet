using LiftNet.Contract.Enums.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Utility.Utils
{
    public static class JobUtil
    {
        public static string GetJobId(JobType type, DateTime? insertTime = null)
        {
            var jobName = GetJobName(type);
            if (insertTime != null)
            {
                return jobName + insertTime.Value.Ticks.ToString();
            }
            return jobName + DateTime.UtcNow.Ticks.ToString();
        }

        public static string GetJobName(JobType type)
        {
            return type switch
            {
                JobType.ProvinceDiscovery => "ProvinceDisc",
                _ => string.Empty,
            };
        }
    }
}
