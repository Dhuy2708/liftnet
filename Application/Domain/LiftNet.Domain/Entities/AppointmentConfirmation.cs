using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("AppointmentConfirmations")]
    public sealed class AppointmentConfirmation
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

        public string? Img
        {
            get; set;
        }

        public string? Content
        {
            get; set;
        }

        public int Status
        {
            get; set;
        }

        public DateTime CreatedAt
        {
            get; set;
        }

        public DateTime ModifiedAt
        {
            get; set;
        } = DateTime.UtcNow;

        public DateTime ExpiredAt
        {
            get; set;
        }

        // mapping
        public Appointment Appointment
        {
            get; set;
        }
    }
}
