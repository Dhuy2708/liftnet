using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("CoachExtensions")]
    public sealed class CoachExtension
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get; set;
        }

        [ForeignKey(nameof(Coach))]
        public string CoachId
        {
            get; set;
        }

        public int SessionTrained
        {
            get; set;
        }

        public int ReviewCount
        {
            get; set;
        }

        public float Star
        {
            get; set;
        }

        // mapping
        public User Coach
        {
            get; set;
        }
    }
}
