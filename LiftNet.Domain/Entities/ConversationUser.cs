using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("ConversationUsers")]
    public sealed class ConversationUser // for group only
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get; set;
        }

        [ForeignKey(nameof(Conversation))]
        public string ConversationId
        {
            get; set;
        }

        [ForeignKey(nameof(User))]
        public string UserId
        {
            get; set;
        }

        public bool IsAdmin // for group
        {
            get; set;
        } = false;

        // mapping
        public Conversation Conversation
        {
            get; set;
        }

        public User User
        {
            get; set;
        }
    }
}
