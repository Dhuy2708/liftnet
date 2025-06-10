using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiftNet.Engine.Engine.Impl
{
    public class ChatbotEngine : IChatBotEngine
    {
        private readonly string _engineUrl;
        private readonly HttpClient _httpClient;

        public ChatbotEngine(string engineUrl)
        {
            _engineUrl = engineUrl.TrimEnd('/');
            _httpClient = new HttpClient()
            {
                Timeout = TimeSpan.FromHours(1)
            };
        }

        public async Task<string> ChatAsync(string userId, string conversationId, string message)
        {
            var requestBody = new
            {
                message = message,
                session_id = conversationId,
                user_id = userId,
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );
                
            var url = $"{_engineUrl}/chat";
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Chat API call failed: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseContent);
            var answer = doc.RootElement
                .GetProperty("response")
                .GetProperty("answer")
                .GetString();

            return answer ?? string.Empty;
        }

    }
}
