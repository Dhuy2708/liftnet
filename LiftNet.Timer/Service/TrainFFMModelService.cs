using LiftNet.Contract.Enums.Job;
using LiftNet.Timer.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Timer.Service
{
    internal class TrainFFMModelService : BaseSystemJob
    {
        public TrainFFMModelService(IServiceProvider provider) : base(JobType.TrainFFMModel, provider)
        {
        }

        protected override Task<JobStatus> KickOffJobServiceAsync()
        {
            throw new NotImplementedException();
        }
    }
}
