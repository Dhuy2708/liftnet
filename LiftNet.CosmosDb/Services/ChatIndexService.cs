using LiftNet.Contract.Constants;
using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.CosmosDb.Contracts;
using LiftNet.Domain.Enums.Indexes;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.CosmosDb.Services
{
    public class ChatIndexService : IndexBaseService<ChatMessageIndexData>, IChatIndexService
    {
        private readonly ILiftLogger<ChatIndexService> _logger;
        public ChatIndexService(CosmosCredential cred, ILiftLogger<ChatIndexService> logger) : base(cred, CosmosContainerId.Chat)
        {
            _logger = logger;
        }

        public async Task<(List<ChatMessageIndexData> datas, string? nextPageToken)> GetMessages(string conversationId, int pageSize = 15, string? nextPageToken = null)
        {
            try
            {
                var condition = new QueryCondition()
                {
                    NextPageToken = nextPageToken,
                    PageSize = pageSize,
                    Sort = new SortCondition()
                    {
                        Name = "created",
                        Type = Contract.Enums.SortType.Desc,
                    }
                };
                condition.AddCondition(new ConditionItem("conversationid", conversationId));

                (var datas, var newNextPageToken) = await QueryAsync(condition);
                return (datas, newNextPageToken);
            }
            catch (Exception e)
            {
                _logger.Error(e, "GetMessages error");
            }
            return ([], null);
        }

        public async Task<int> SaveMessages(string conversationId, string userId, string message, ChatMessageType type = ChatMessageType.Text)
        {
            try
            {
                var index = new ChatMessageIndexData()
                {
                    Id = Guid.NewGuid().ToString(),
                    ConversationId = conversationId,
                    UserId = userId,
                    Message = message,
                    Type = type,
                    Schema = DataSchema.Chat,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };

                await UpsertAsync(index);
                return 1;
            }
            catch (Exception e)
            {
                _logger.Error(e, "SaveMessages error");
            }
            return 0;
        }
    }
}
