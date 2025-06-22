using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("Feedbacks")]
    public sealed class Feedback
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

        [ForeignKey(nameof(Reviewer))]
        public string ReviewerId
        {
            get; set;
        }

        [ForeignKey(nameof(Coach))]
        public string CoachId
        {
            get; set;
        }

        public int Star
        {
            get; set;
        }

        public string? Medias // serialize list<string>
        {
            get; set;
        }

        public string? Content
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
        
        public User Coach
        {
            get; set;
        }
    }
}
