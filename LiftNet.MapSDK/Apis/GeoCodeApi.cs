using LiftNet.MapSDK.Contracts;
using LiftNet.MapSDK.Contracts.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiftNet.MapSDK.Apis
{
    public class GeoCodeApi : BaseApi
    {
        private const string BaseUrl = "https://rsapi.goong.io/geocode";

        public GeoCodeApi(MapApiKey key) : base(key.Key, BaseUrl)
        {
        }

        public async Task<List<ForwardGeocodeResult>?> FowardGeoCodeAsync(string address)
        {
            var url = $"{_baseUrl}?address={Uri.EscapeDataString(address)}&api_key={_apiKey}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var res = JsonSerializer.Deserialize<ForwardGeocodeRes>(json, options);

            return res?.Results;
        }
    }
}
