using LiftNet.Contract.Dtos;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Views;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.MapSDK.Apis;
using LiftNet.MapSDK.Contracts.Res;
using LiftNet.Utility.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace LiftNet.Service.Services
{
    public class GeoService : IGeoService
    {
        private readonly ILiftLogger<GeoService> _logger;
        private readonly IUnitOfWork _uow;
        private readonly AutocompleteApi _autocompleteApi;
        private readonly GeoCodeApi _geoCodeApi;
        private readonly PlaceApi _placeApi;

        public GeoService(ILiftLogger<GeoService> logger,
                          IUnitOfWork uow,
                          AutocompleteApi autocompleteApi,
                          GeoCodeApi geoCodeApi,
                          PlaceApi placeApi) // Inject PlaceApi
        {
            _logger = logger;
            _uow = uow;
            _autocompleteApi = autocompleteApi;
            _geoCodeApi = geoCodeApi;
            _placeApi = placeApi;
        }

        private string NormalizeVietnamese(string input)
        {
            input = input.Replace("đ", "d").Replace("Đ", "D");

            return string.Concat(input.Normalize(NormalizationForm.FormD)
                                      .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark))
                         .ToLower();
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
                var normalizedQuery = NormalizeVietnamese(q);
                var provinces = await _uow.ProvinceRepo.GetQueryable()
                                                       .ToListAsync(); // Fetch data into memory

                provinces = provinces.Where(x => NormalizeVietnamese(x.Name).Contains(normalizedQuery))
                                     .ToList(); // Apply filtering in memory

                return provinces.Select(x => x.ToDto()).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "error while searching provinces");
            }
            return new List<ProvinceDto>();
        }

        public async Task<List<DistrictDto>> SearchDistrictsAsync(int provinceCode, string q)
        {
            try
            {
                var normalizedQuery = NormalizeVietnamese(q);
                var districts = await _uow.DistrictRepo.GetQueryable()
                                                       .Where(x => x.ProvinceCode == provinceCode)
                                                       .ToListAsync(); // Fetch data into memory

                districts = districts.Where(x => NormalizeVietnamese(x.Name).Contains(normalizedQuery))
                                     .ToList(); // Apply filtering in memory

                return districts.Select(x => x.ToDto()).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "error while searching districts");
            }
            return new List<DistrictDto>();
        }

        public async Task<List<WardDto>> SearchWardsAsync(int provinceCode, int districtCode, string q)
        {
            try
            {
                var normalizedQuery = NormalizeVietnamese(q);
                var wards = await _uow.WardRepo.GetQueryable()
                                               .Where(x => x.District.ProvinceCode == provinceCode &&
                                                           x.DistrictCode == districtCode)
                                               .ToListAsync(); // Fetch data into memory

                wards = wards.Where(x => NormalizeVietnamese(x.Name).Contains(normalizedQuery))
                             .ToList(); // Apply filtering in memory

                return wards.Select(x => x.ToDto()).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "error while searching wards");
            }
            return new List<WardDto>();
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

        public async Task<(double lat, double lng)> GetCoordinatesByProvinceCodeAsync(int provinceCode)
        {
            try
            {
                var province = await _uow.ProvinceRepo.GetQueryable()
                    .Where(x => x.Code == provinceCode)
                    .Select(x => new { x.Latitude, x.Longitude })
                    .FirstOrDefaultAsync();

                if (province == null)
                {
                    _logger.Error($"Province with code {provinceCode} not found.");
                    return (0, 0);
                }

                return (province.Latitude, province.Longitude);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while fetching coordinates.");
                return (0, 0);
            }
        }

        public async Task<List<PlacePredictionView>> AutocompleteSearchAsync(string input, double? latitude = null, double? longitude = null)
        {
            try
            {
                var predictions = await _autocompleteApi.GetAutocompleteAsync(input, latitude, longitude);

                if (predictions == null)
                {
                    _logger.Error("No predictions found.");
                    return new List<PlacePredictionView>();
                }

                return predictions.Select(p => new PlacePredictionView
                {
                    Description = p.Description,
                    PlaceId = p.PlaceId,
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while fetching autocomplete predictions.");
                return new List<PlacePredictionView>();
            }
        }

        public async Task<string> ReverseGeoCodeAsync(double latitude, double longitude)
        {
            try
            {
                var result = await _geoCodeApi.ReverseGeoCodeAsync(latitude, longitude);
                if (result == null || !result.Any())
                {
                    _logger.Error($"No reverse geocode result found for coordinates: {latitude}, {longitude}");
                    return string.Empty;
                }

                return result.FirstOrDefault()?.FormattedAddress ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error while reverse geocoding coordinates: {latitude}, {longitude}");
                return string.Empty;
            }
        }

        public async Task<string> GetPlaceNameAsync(string placeId)
        {
            try
            {
                var placeDetail = await _placeApi.GetPlaceDetailAsync(placeId);
                if (placeDetail == null)
                {
                    _logger.Error($"No place detail found for placeId: {placeId}");
                    return string.Empty;
                }

                return placeDetail.Name;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error while fetching place name for placeId: {placeId}");
                return string.Empty;
            }
        }
    }
}
