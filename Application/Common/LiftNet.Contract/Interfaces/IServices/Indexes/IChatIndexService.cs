﻿using LiftNet.Domain.Enums.Indexes;
using LiftNet.Domain.Indexes;
using LiftNet.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IServices.Indexes
{
    public interface IChatIndexService : IIndexBaseService<ChatMessageIndexData>, IDependency
    {
        Task<(List<ChatMessageIndexData> datas, string? nextPageToken)> GetMessages(string conversationId, int pageSize = 15, string? nextPageToken = null);
        Task<string?> SaveMessages(string conversationId, string senderId, string message, ChatMessageType type = ChatMessageType.Text, DateTime? time = null); 
        Task<ChatMessageIndexData> GetLastMessage(string conversationId);
    }
}
