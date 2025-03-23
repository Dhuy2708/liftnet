using LiftNet.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("Appointments")]
    public sealed class Appointment : IEntity
    {
        [Key]
        public string Id
        {
            get; set;
        } = Guid.NewGuid().ToString();

        [ForeignKey(nameof(Booker))]
        public string? BookerId
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public string Address
        {
            get; set;
        }

        public DateTime StartTime
        {
            get; set;
        }

        public DateTime EndTime
        {
            get; set;
        }

        public int Status
        {
            get; set;
        }

        public int RepeatingType
        {
            get; set;
        }

        public DateTime Created
        {
            get; set;
        }

        public DateTime Modified
        {
            get; set;
        } = DateTime.UtcNow;

        // mapping
        public User? Booker
        {
            get; set;
        }

        public ICollection<AppointmentParticipant> Participants
        {
            get; set;
        }
    }
}
