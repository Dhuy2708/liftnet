using LiftNet.Contract.Dtos;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Views;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.MapSDK.Apis;
using LiftNet.Utility.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Service.Services
{
    public class GeoService : IGeoService
    {
        private readonly ILiftLogger<GeoService> _logger;
        private readonly IUnitOfWork _uow;
        private readonly AutocompleteApi _autocompleteApi;
        private readonly GeoCodeApi _geoCodeApi;

        public GeoService(ILiftLogger<GeoService> logger,
                          IUnitOfWork uow,
                          AutocompleteApi autocompleteApi,
                          GeoCodeApi geoCodeApi)
        {
            _logger = logger;
            _uow = uow;
            _autocompleteApi = autocompleteApi;
            _geoCodeApi = geoCodeApi;
        }

        public async Task<List<Province>> GetAllDivisionsAsync()
        {
            try
            {
                var queryable = _uow.ProvinceRepo.GetQueryable();
                queryable = queryable.Include(x => x.Districts)
                                     .ThenInclude(x => x.Wards);
                var result = await queryable.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"error while getting all divisions");
            }
            return [];
        }

        public async Task<List<ProvinceDto>> SearchProvincesAsync(string q)
        {
            try
            {
                var queryable = _uow.ProvinceRepo.GetQueryable();

                var provinces = await queryable.Where(x => x.Name.Contains(q))
                                               .ToListAsync();

                return provinces.Select(x => x.ToDto()).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "error while searching provinces");
            }
            return null;
        }

        public async Task<List<DistrictDto>> SearchDistrictsAsync(int provinceCode, string q)
        {
            try
            {
                var queryable = _uow.DistrictRepo.GetQueryable();

                var districits = await queryable.Where(x => x.ProvinceCode == provinceCode &&
                                                            x.Name.Contains(q))
                                               .ToListAsync();

                return districits.Select(x => x.ToDto()).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "error while searching districts");
            }
            return null;
        }

        public async Task<List<WardDto>> SearchWardsAsync(int provinceCode, int districtCode, string q)
        {
            try
            {
                var queryable = _uow.WardRepo.GetQueryable();

                var wards = await queryable.Where(x => x.District.ProvinceCode == provinceCode &&
                                                            x.DistrictCode == districtCode &&
                                                            x.Name.Contains(q))
                                               .ToListAsync();

                return wards.Select(x => x.ToDto()).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "error while searching wards");
            }
            return [];
        }

        public async Task<(double lat, double lng)> FowardGeoCodeAsync(string address)
        {
            try
            {
                var result = await _geoCodeApi.FowardGeoCodeAsync(address);
                if (result == null)
                {
                    return default;
                }
                var mostPotentialResult = result.FirstOrDefault();
                if (mostPotentialResult == null)
                {
                    return default;
                }

                var lat = mostPotentialResult!.Geometry.Location.Lat;
                var lng = mostPotentialResult!.Geometry.Location.Lng;
                return (lat, lng);  
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"error while foward geocode address: {address}");
            }
            return default;
        }
    }
}
