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

        [ForeignKey(nameof(Appointment))]
        public string? AppointmentId
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

        [ForeignKey(nameof(FromUser))]
        public string? FromUserId
        {
            get; set;
        }

        [ForeignKey(nameof(ToUser))]
        public string? ToUserId
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

        // mapping
        public User User
        {
            get; set;
        }

        public User FromUser
        {
            get; set;
        }

        public User ToUser
        {
            get; set;
        }

        public Appointment Appointment
        {
            get; set;
        }
    }
}
