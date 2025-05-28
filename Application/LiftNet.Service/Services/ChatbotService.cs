using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Domain.Interfaces;
using LiftNet.Engine.Engine;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Service.Services
{
    public class ChatbotService : IChatbotService
    {
        private readonly IChatBotEngine _chatEngine;
        private readonly IChatBotConversationRepo _conversationRepo;
        private readonly ILiftLogger<ChatbotService> _logger;

        public ChatbotService(IChatBotEngine chatEngine, IChatBotConversationRepo conversationRepo, ILiftLogger<ChatbotService> logger)
        {
            _chatEngine = chatEngine;
            _conversationRepo = conversationRepo;
            _logger = logger;
        }

        public async Task<string> ChatAsync(string userId, string conversationId, string message)
        {
            try
            {
                return await _chatEngine.ChatAsync(userId, conversationId, message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "error occured while sending chat bot message");
                throw new Exception("An error occurred while processing the chat request.", ex);
            }
        }

        public async Task<bool> CheckConversationAsync(string userId, string conversationId)
        {
            try
            {
                var isExist = await _conversationRepo.GetQueryable()
                                                          .AnyAsync(x => x.Id == conversationId &&
                                                                         x.UserId == userId);
                return isExist;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "error occured while checking chat bot conversation");
                throw new Exception("An error occurred while checking the conversation.", ex);
            }
        }
    }
}
