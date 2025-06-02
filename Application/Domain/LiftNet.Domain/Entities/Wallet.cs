using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("Wallets")]
    public sealed class Wallet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id
        {
            get; set;
        } = Guid.NewGuid().ToString();

        [ForeignKey(nameof(User))]
        public string UserId
        {
            get; set;
        }

        public double Balance
        {
            get; set;
        }

        public DateTime LastUpdate
        {
            get; set;
        } = DateTime.UtcNow;

        // mapping
        public User User
        {
            get; set;
        }
    }
}
