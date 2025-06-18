using LiftNet.Contract.Dtos.Chatbot;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.ChatBots.Commands.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiftNet.Handler.ChatBots.Commands
{
    public class CreateChatBotConversationHandler : IRequestHandler<CreateChatBotConversationCommand, LiftNetRes<CreateChatbotConversationResponse>>
    {
        private readonly ILiftLogger<CreateChatBotConversationHandler> _logger;
        private readonly IChatBotConversationRepo _conversationRepo;
        private HttpClient httpClient;

        public CreateChatBotConversationHandler(ILiftLogger<CreateChatBotConversationHandler> logger, IChatBotConversationRepo conversationRepo)
        {
            _logger = logger;
            _conversationRepo = conversationRepo;
            httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(60),
                BaseAddress = new Uri("http://127.0.0.1:5000/")
            };
        }

        public async Task<LiftNetRes<CreateChatbotConversationResponse>> Handle(CreateChatBotConversationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var title = await GetConversationTitleAsync(request.FirstPrompt, cancellationToken);

                var entity = new ChatBotConversation()
                {
                    Id = Guid.NewGuid().ToString(),
                    LastUpdate = DateTime.UtcNow,
                    Title = title ?? request.FirstPrompt,
                    UserId = request.UserId,
                };

                await _conversationRepo.Create(entity);
                var result = new CreateChatbotConversationResponse
                {
                    Id = entity.Id,
                    Title = entity.Title,
                };
                return LiftNetRes<CreateChatbotConversationResponse>.SuccessResponse(result, message: "Conversation created successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating chat bot conversation");
                return LiftNetRes<CreateChatbotConversationResponse>.ErrorResponse("An error occurred while creating the conversation");
            }
        }

        private async Task<string?> GetConversationTitleAsync(string question, CancellationToken cancellationToken)
        {
            try
            {
                var payload = new { question };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("api/generate-title", content, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var err = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.Warn($"Flask API error: {err}");
                    return null;
                }

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                using var doc = JsonDocument.Parse(responseContent);
                return doc.RootElement.GetProperty("title").GetString();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get title from Flask API");
                return null;
            }
        }

    }
}
