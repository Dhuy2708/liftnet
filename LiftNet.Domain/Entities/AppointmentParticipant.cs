using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("AppointmentParticipants")]
    public sealed class AppointmentParticipant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get; set;
        }

        [ForeignKey(nameof(Appointment))]
        public string AppointmentId
        {
            get; set;
        }

        [ForeignKey(nameof(User))]
        public string UserId
        {
            get; set;
        }

        public bool IsBooker
        {
            get; set;
        }

        // mapping
        public Appointment Appointment
        {
            get; set;
        }

        public User User
        {
            get; set;
        }
    }
}
