using LiftNet.Ioc;
using LiftNet.MapSDK.Contracts;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiftNet.MapSDK.Apis
{
    public class AutocompleteApi : IDependency
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _apiKey;
        public AutocompleteApi(MapApiKey key)
        {
            _baseUrl = "https://rsapi.goong.io/place/autocomplete";
            _apiKey = key.Key;

            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://provinces.open-api.vn/api/")
            };
        }

        public async Task<List<Prediction>?> GetAutocompleteAsync(string input, double? latitude = null, double? longitude = null, int limit = 10, int radius = 10)
        {
            var url = $"{_baseUrl}?input={Uri.EscapeDataString(input)}&limit={limit}&radius={radius}&api_key={_apiKey}";

            if (latitude.HasValue && longitude.HasValue)
            {
                url += $"&location={latitude.Value},{longitude.Value}";
            }

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AutocompleteRes>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });
            return result?.Predictions;
        }
    }
}
