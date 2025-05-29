using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("Transactions")]
    public class Transaction
    {
        [Key]
        public string Id
        {
            get; set;
        } = Guid.NewGuid().ToString();

        public long? PaymentId
        {
            get; set;
        }

        public string? TransactionId // for external payment system
        {
            get; set;
        }

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

        public int Type
        {
            get; set;
        } // 1: Topup, 2: Transfer, 3: Withdraw

        public int? PaymentMethod
        {
            get; set;
        }

        public int Status
        {
            get; set;
        }

        public string? Additional
        {
            get; set;
        }

        public DateTime CreatedAt
        {
            get; set;
        } = DateTime.UtcNow;

        public DateTime? TimeToLive
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
