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

        public string? TransactionId
        {
            get; set;
        } = string.Empty;

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

        // mapping
        public User User
        {
            get; set;
        }
    }
}
