using LiftNet.ProvinceSDK.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiftNet.ProvinceSDK.Apis
{
    public class BaseApi
    {
        private readonly HttpClient _httpClient;
        public BaseApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://provinces.open-api.vn/api/");
        }

        public async Task<string> GetVersionAsync()
        {
            var response = await _httpClient.GetAsync("version/");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiVersion>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result?.DataVersion ?? string.Empty;
        }
    }
}
