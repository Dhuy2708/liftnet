using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("CoachRecommendations")]
    public sealed class CoachRecommendation
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

        [ForeignKey(nameof(Seeker))]
        public string SeekerId
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public DateTime LastUpdated
        {
            get; set;
        }
        

        // mapping
        public User Coach
        {
            get; set;
        }

        public User Seeker
        {
            get; set;
        }
    }
}
