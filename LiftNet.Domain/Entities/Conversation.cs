using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("Conversations")]
    public sealed class Conversation
    {
        [Key]
        public string Id
        {
            get; set;
        } = Guid.NewGuid().ToString();

        public bool IsGroup
        {
            get; set;
        }
        public string? GroupTitle
        {
            get; set;
        }
        public string? GroupImage
        {
            get; set;
        }
        public DateTime CreatedAt
        {
            get; set;
        }
        public DateTime LastUpdate
        {
            get; set;
        } = DateTime.UtcNow;
    }
}
