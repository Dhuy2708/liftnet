using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("AppointmentFeedbacks")]
    public sealed class AppointmentFeedback
    {
        [Key]
        public int Id
        {
            get; set;
        }

        [ForeignKey(nameof(Appointment))]
        public string ApppointmentId
        {
            get; set;
        }

        public string ReviewerId
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

        public int Star
        {
            get; set;
        }

        // mapping
        public Appointment Appointment
        {
            get; set;
        }

        public User Reviewer
        {
            get; set;
        }
    }
}
