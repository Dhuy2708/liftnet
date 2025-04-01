using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Timer.Service.Common
{
    public abstract class BaseJob : IJob
    {
        protected readonly IServiceProvider _provider;

        public BaseJob(IServiceProvider provider)
        {
            _provider = provider;
        }

        public abstract Task Execute(IJobExecutionContext context);
    }
}
