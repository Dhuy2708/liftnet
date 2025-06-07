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

        [ForeignKey(nameof(User))]
        public string UserId
        {
            get; set;
        }

        public int SenderType
        {
            get; set;
        }

        public int RecieverType
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
        public User User
        {
            get; set;
        }
    }
}
