using LiftNet.Contract.Constants;
using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Enums;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.CosmosDb.Contracts;
using LiftNet.Domain.Enums.Indexes;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Interfaces;
using LiftNet.Utility.Extensions;
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

        public async Task<ChatMessageIndexData> GetLastMessage(string conversationId)
        {
            try
            {
                var condition = new QueryCondition()
                {
                    PageSize = 1,
                    Sort = new SortCondition()
                    {
                        Name = "created",
                        Type = Contract.Enums.SortType.Desc,
                    }
                };
                condition.AddCondition(new ConditionItem("conversationid", conversationId));
                condition.AddCondition(new ConditionItem(DataSchema.Chat, logic: QueryLogic.And));
                (var datas, _) = await QueryAsync(condition);
                if (datas.IsNotNullOrEmpty())
                {
                    return datas.First();
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.Error(e, "GetLastMessage error");
                throw;
            }
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

        public async Task<string?> SaveMessages(string conversationId, string senderId, string message, ChatMessageType type = ChatMessageType.Text, DateTime? time = null)
        {
            try
            {
                var msgId = Guid.NewGuid().ToString();
                var index = new ChatMessageIndexData()
                {
                    Id = msgId,
                    ConversationId = conversationId,
                    UserId = senderId,
                    Message = message,
                    Type = type,
                    Schema = DataSchema.Chat,
                    CreatedAt = time != null ? time.Value : DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };

                await UpsertAsync(index);
                return msgId;
            }
            catch (Exception e)
            {
                _logger.Error(e, "SaveMessages error");
            }
            return null;
        }

    }
}
