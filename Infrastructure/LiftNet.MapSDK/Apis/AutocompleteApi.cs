using LiftNet.Ioc;
using LiftNet.MapSDK.Contracts;
using LiftNet.MapSDK.Contracts.Res;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiftNet.MapSDK.Apis
{
    public class AutocompleteApi : BaseApi, IDependency
    {
        private const string BaseUrl = "https://rsapi.goong.io/place/autocomplete";
        public AutocompleteApi(MapApiKey key) : base (key.Key, BaseUrl)
        {
        }

        public async Task<List<Prediction>?> GetAutocompleteAsync(string input, double? latitude = null, double? longitude = null, int limit = 10, int radius = 10)
        {
            var url = $"{_baseUrl}?input={Uri.EscapeDataString(input)}&limit={limit}&radius={radius}&api_key={_apiKey}";

            if (latitude.HasValue && longitude.HasValue)
            {
                url += $"&location={latitude.Value},{longitude.Value}";
            }

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AutocompleteRes>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });
            return result?.Predictions;
        }
    }
}
