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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id
        {
            get; set;
        } = Guid.NewGuid().ToString();

        [ForeignKey(nameof(User))]
        public string? ClientId
        {
            get; set;
        }

        [ForeignKey(nameof(User))]        
        public string? CoachId           
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

        // mapping
        public User? Client
        {
            get; set;
        }

        public User? Coach
        {
            get; set;
        }
    }
}
