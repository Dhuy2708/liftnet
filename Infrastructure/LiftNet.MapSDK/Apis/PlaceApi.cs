using LiftNet.Ioc;
using LiftNet.MapSDK.Contracts;
using LiftNet.MapSDK.Contracts.Res;
using System.Text.Json;

namespace LiftNet.MapSDK.Apis
{
    public class PlaceApi : BaseApi, IDependency
    {
        private const string BaseUrl = "https://rsapi.goong.io/Place/Detail";

        public PlaceApi(MapApiKey key) : base(key.Key, BaseUrl)
        {
        }

        public async Task<PlaceDetailResult?> GetPlaceDetailAsync(string placeId)
        {
            var url = $"{_baseUrl}?place_id={Uri.EscapeDataString(placeId)}&api_key={_apiKey}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
            var res = JsonSerializer.Deserialize<PlaceDetailRes>(json, options);

            return res?.Result;
        }
    }
}

