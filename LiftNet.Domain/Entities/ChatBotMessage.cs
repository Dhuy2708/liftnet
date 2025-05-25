using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("ChatBotMessages")]
    public sealed class ChatBotMessage
    {
        public string Id
        {
            get; set;
        } = Guid.NewGuid().ToString();

        [ForeignKey(nameof(ChatBotConversation))]
        public string ConversationId
        {
            get; set;
        }

        public string Message
        {
            get; set;
        }

        public DateTime Time
        {
            get; set;
        } = DateTime.UtcNow;

        public bool IsHuman
        {
            get; set;
        }

        public string? AdditionalData
        {
            get; set;
        }

        // mapping
        public ChatBotConversation ChatBotConversation
        {
            get; set;
        }
    }
}
