using LiftNet.Contract.Enums.Job;
using LiftNet.Domain.Interfaces;
using LiftNet.Engine.Engine;
using LiftNet.Timer.Service.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Timer.Service
{
    public class SocialScoreService : BaseSystemJob
    {
        private ILiftLogger<SocialScoreService> _logger => _provider.GetRequiredService<ILiftLogger<SocialScoreService>>();
        private ISocialEngine _socialEngine => _provider.GetRequiredService<ISocialEngine>();
        public SocialScoreService(IServiceProvider provider) : base(JobType.AllSocialScoreUp, provider, TimeSpan.FromHours(2))
        {
        }

        protected override async Task<JobStatus> KickOffJobServiceAsync()
        {
            try
            {
                _logger.Info("begin to compute all social scores");
                await _socialEngine.ComputeAllUserScores();
                return JobStatus.Finished;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while computing social scores");
                return JobStatus.Failed;
            }
        }
    }
}
