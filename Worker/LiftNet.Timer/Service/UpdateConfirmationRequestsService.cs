using LiftNet.Contract.Enums.Job;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Interfaces;
using LiftNet.Timer.Service.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Timer.Service
{
    public class UpdateConfirmationRequestsService : BaseSystemJob
    {
        private ILiftLogger<UpdateConfirmationRequestsService> _logger => _provider.GetRequiredService<ILiftLogger<UpdateConfirmationRequestsService>>();
        private IUnitOfWork _uow => _provider.GetRequiredService<IUnitOfWork>();

        public UpdateConfirmationRequestsService(IServiceProvider provider) 
                : base(JobType.UpdateConfirmationRequest, provider, TimeSpan.FromMinutes(1))
        {
        }

        protected override async Task<JobStatus> KickOffJobServiceAsync()
        {
            try
            {
                _logger.Info("begin to handle expired confirmation requests");
                return JobStatus.Finished;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while handling expired confirmation requests");
                return JobStatus.Failed;
            }
        }
    }
}
