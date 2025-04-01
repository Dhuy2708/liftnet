using LiftNet.Contract.Constants;
using LiftNet.Contract.Enums.Job;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Interfaces;
using LiftNet.ProvinceSDK.Apis;
using LiftNet.Timer.Service.Common;
using LiftNet.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Timer.Service
{
    public class ProvinceDiscService : BaseSystemJob
    {
        private ILiftLogger<ProvinceDiscService> _logger => _provider.GetRequiredService<ILiftLogger<ProvinceDiscService>>(); 
        private ProvinceApi _api => _provider.GetRequiredService<ProvinceApi>(); 
        private IVersionRepo _versionRepo => _provider.GetRequiredService<IVersionRepo>();

        public ProvinceDiscService(IServiceProvider provider) : base(JobType.ProvinceDiscovery, provider, TimeSpan.FromHours(1))
        {
        }

        protected override async Task<JobStatus> KickOffJobServiceAsync()
        {
            try
            {
                _logger.Info("begin province disc service");

                var oldVersion = await _versionRepo.GetQueryable().FirstOrDefaultAsync(x => x.Key == LiftNetVersionKeys.VN_GEO);
                var newVersion = await _api.GetVersionAsync();
                
                if (oldVersion != null && oldVersion.Value.Eq(newVersion))
                {
                    _logger.Info("province version is up to date, skip");
                    return JobStatus.Finished;
                }

                var allDivisions = await _api.GetAllDivisionsJson();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "error while discover provinces");
            }
            return JobStatus.Failed;
        }
    }
}
