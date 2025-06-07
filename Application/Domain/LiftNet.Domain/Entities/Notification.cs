using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("Notifications")]
    public sealed class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get; set;
        }

        [ForeignKey(nameof(Reciever))]
        public string RecieverId // the current user
        {
            get; set;
        }

        public int RecieverType
        {
            get; set;
        }

        [ForeignKey(nameof(Sender))]
        public string? SenderId
        {
            get; set;
        }

        public int SenderType
        {
            get; set;
        }

        public string? Title
        {
            get; set;
        }

        public string? Body
        {
            get; set;
        } = string.Empty;

        public DateTime CreatedAt
        {
            get; set;
        } = DateTime.UtcNow;

        public int EventType
        {
            get; set;
        }

        public int ReferenceLocation
        {
            get; set;
        }

        // mapping
        public User Sender
        {
            get; set;
        }
        public User Reciever
        {
            get; set;
        }
    }
}
