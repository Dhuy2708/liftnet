﻿using LiftNet.Contract.Dtos;
using LiftNet.Contract.Views;
using LiftNet.Domain.Entities;
using LiftNet.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IServices
{
    public interface IGeoService : IDependency
    {
        Task<List<Province>> GetAllDivisionsAsync();
        Task<List<ProvinceDto>> SearchProvincesAsync(string q);
        Task<List<DistrictDto>> SearchDistrictsAsync(int provinceCode, string q);
        Task<List<WardDto>> SearchWardsAsync(int provinceCode, int districtCode, string q);
        Task<(double lat, double lng)> FowardGeoCodeAsync(string address);
        Task<(double lat, double lng)> GetCoordinatesByProvinceCodeAsync(int provinceCode);
        Task<List<PlacePredictionView>> AutocompleteSearchAsync(string input, double? latitude = null, double? longitude = null);
        Task<string> ReverseGeoCodeAsync(double latitude, double longitude);
        Task<string> GetPlaceNameAsync(string placeId);
        Task<PlaceDetailDto?> GetPlaceDetailAsync(string placeId);
    }
}
