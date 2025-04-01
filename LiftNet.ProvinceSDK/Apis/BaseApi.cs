using LiftNet.Ioc;
using LiftNet.ProvinceSDK.Contracts;
using System;
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
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiVersion>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result?.DataVersion ?? string.Empty;
        }

        public async Task<string> GetAllDivisionsJson()
        {
            var response = await _httpClient.GetAsync("");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return json;
        }
    }
}
