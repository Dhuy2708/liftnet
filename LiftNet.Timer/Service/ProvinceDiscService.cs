using LiftNet.Contract.Constants;
using LiftNet.Contract.Enums.Job;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
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
        private IUnitOfWork _uow => _provider.GetRequiredService<IUnitOfWork>();
        private IGeoService _geoService => _provider.GetRequiredService<IGeoService>();

        public ProvinceDiscService(IServiceProvider provider) : base(JobType.ProvinceDiscovery, provider)
        {  
        }

        protected override async Task<JobStatus> KickOffJobServiceAsync()
        {
            try
            {
                _logger.Info("begin province disc service");
                var oldVersion = await _uow.VersionRepo.GetQueryable().FirstOrDefaultAsync(x => x.Key == LiftNetVersionKeys.VN_GEO);
                var newVersion = await _api.GetVersionAsync();
                
                if (newVersion.IsNullOrEmpty())
                {
                    _logger.Error("failed to get province api version");
                    return JobStatus.Failed;
                }
                if (oldVersion != null && oldVersion.Value.Eq(newVersion))
                {
                    _logger.Info("province version is up to date, skip");
                    return JobStatus.Finished;
                }

                _ = await _uow.VersionRepo.Create(new Domain.Entities.LiftNetVersion()
                {
                    Key = LiftNetVersionKeys.VN_GEO,
                    Value = newVersion
                });
                var allDivisions = await _api.GetAllDivisionsJson();
                if (allDivisions.IsNullOrEmpty())
                {
                    _logger.Error("failed to get provinces");
                    return JobStatus.Failed;
                }

                foreach (var province in allDivisions!)
                {
                    await Task.Delay(500);
                    (var lat, var lng) = await _geoService.FowardGeoCodeAsync(province.Name);
                    if (lat == 0 || lng == 0)
                    {
                        _logger.Error($"error while getting lat and lng of province: {province.Name}");
                    }
                    province.Latitude = lat;
                    province.Longitude = lng;
                    foreach (var district in province.Districts)
                    {
                        district.ProvinceCode = province.Code;
                        foreach (var ward in district.Wards)
                        {
                            ward.DistrictCode = district.Code;
                        }
                    }
                }
                _ = await _uow.ProvinceRepo.UpdateRange(allDivisions);
                var result = await _uow.CommitAsync();

                if (result > 0)
                {
                    return JobStatus.Finished;
                }
                else
                {
                    return JobStatus.Failed;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "error while discover provinces");
                await _uow.RollbackAsync();
                return JobStatus.Failed;
            }
        }

        private async Task DeleteOldData()
        {
            var oldProvinces = await _uow.ProvinceRepo.GetQueryable().ToListAsync();
            if (oldProvinces.IsNullOrEmpty())
            {
                return;
            }
            await _uow.ProvinceRepo.HardDeleteRange(oldProvinces);
        }
    }
}
