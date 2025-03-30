using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Enums.Job
{
    public enum JobType
    {
        None = 0,
        ProvinceDiscovery = 1,
    }

    public enum JobKind
    {
        None = 0,
        Action = 1,
        System = 2,
        Customer = 3,
    }

    public enum JobStatus
    {
        None = 0,
        Waiting = 1,
        InProgress = 2,
        Finished = 3,
        Failed = 4,
    }

    public static class ActionJobFamily
    {
        private static readonly HashSet<JobType> types =
        [
        ];
        public static bool Contains(JobType type)
        {
            return types.Contains(type);
        }
    }

    public static class SystemJobFamily
    {
        private static readonly HashSet<JobType> types =
        [
            JobType.ProvinceDiscovery,
        ];
        public static bool Contains(JobType type)
        {
            return types.Contains(type);
        }
    }

    public static class CustomerJobFamily
    {
        private static readonly HashSet<JobType> types =
        [
        ];
        public static bool Contains(JobType type)
        {
            return types.Contains(type);
        }
    }
}
