using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("LiftNetTransactions")]
    public sealed class LiftNetTransaction
    {
        [Key]
        public string Id
        {
            get; set;
        } = Guid.NewGuid().ToString();

        [ForeignKey(nameof(User))]
        public string UserId
        {
            get; set;
        }
        
        public double Amount
        {
            get; set;
        }

        public string Description
        {
            get; set;
        } = string.Empty;

        public int Status
        {
            get; set;
        }

        public string? Additional
        {
            get; set;
        }

        public string? FromUserId
        {
            get; set;
        }

        public string? ToUserId
        {
            get; set;
        }

        public DateTime CreatedAt
        {
            get; set;
        } = DateTime.UtcNow;

        // mapping
        public User User
        {
            get; set;
        }

        // mapping
        public User FromUser
        {
            get; set;
        }

        // mapping
        public User ToUser
        {
            get; set;
        }
    }
}
