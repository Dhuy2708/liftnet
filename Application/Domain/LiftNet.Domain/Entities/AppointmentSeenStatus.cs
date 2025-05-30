using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("AppointmentSeenStatuses")]
    public sealed class AppointmentSeenStatus
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

        [ForeignKey(nameof(Appointment))]
        public string AppointmentId
        {
            get; set;
        }

        public int NotiCount
        {
            get; set;
        }

        public DateTime? LastSeen
        {
            get; set;
        }

        public DateTime LastUpdate
        {
            get; set;
        }

        // mapping
        public User User
        {
            get; set;
        }

        public Appointment Appointment
        {
            get; set;
        }
    }
}
