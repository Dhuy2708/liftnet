using LiftNet.ExerciseSDK.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.ExerciseSDK.Core
{
    public class ExerciseApiClient
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://exercisedb.p.rapidapi.com/exercises?limit=9999999&offset=0";
        private readonly string _apiKey;

        public ExerciseApiClient(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException(nameof(apiKey));

            _apiKey = apiKey;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-key", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-host", "exercisedb.p.rapidapi.com");
        }

        public async Task<List<ExerciseRes>> GetAllExercisesAsync()
        {
            var response = await _httpClient.GetAsync(BaseUrl);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API error: {response.StatusCode} - {error}");
            }

            return await response.Content.ReadFromJsonAsync<List<ExerciseRes>>() ?? new List<ExerciseRes>();
        }
    }
}
