using LiftNet.Contract.Enums.Job;
using LiftNet.Contract.Interfaces.IJobs;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly ILiftLogger<ProvinceDiscoveryService> _logger;

        public ProvinceDiscoveryService(IServiceProvider provider) : base(JobType.ProvinceDiscovery, provider)
        {
            _logger = provider.GetRequiredService<ILiftLogger<ProvinceDiscoveryService>>();
        }

        public override async Task KickOffAsync()
        {
            try
            {
                if (!await CheckJobCanRun(IntervalTime))
                {
                    _logger.Warn("job has been run, skip");
                    return;
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
