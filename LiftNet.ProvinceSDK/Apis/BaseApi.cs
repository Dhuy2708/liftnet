using LiftNet.Domain.Entities;
using LiftNet.Ioc;
using LiftNet.ProvinceSDK.Contracts;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiftNet.ProvinceSDK.Apis
{
    public class BaseApi : IDependency
    {
        private readonly HttpClient _httpClient;
        public BaseApi()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://provinces.open-api.vn/api/")
            };
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "User-Agent-Here");
        }

        public async Task<string> GetVersionAsync()
        {
            var response = await _httpClient.GetAsync("version/");
            if (!response.IsSuccessStatusCode)
                return string.Empty;

            var json = await response.Content.ReadAsStringAsync();
            var apiVersion = JsonSerializer.Deserialize<ApiVersion>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });
            return apiVersion?.DataVersion ?? string.Empty;
        }

        public async Task<List<Province>?> GetAllDivisionsJson()
        {
            string urlWithParams = QueryHelpers.AddQueryString(string.Empty, "depth", "3");
            var response = await _httpClient.GetAsync(urlWithParams);
            if (!response.IsSuccessStatusCode)
                return null;
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<Province>>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower 
            });
            return result ?? [];
        }
    }
}
