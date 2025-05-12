using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("SocialSimilarityScores")]
    public sealed class SocialSimilarityScore
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get; set;
        }

        [ForeignKey(nameof(User1))]
        public string UserId1
        {
            get; set;
        } = string.Empty;

        [ForeignKey(nameof(User2))]
        public string UserId2
        {
            get; set;
        }

        public float Score
        {
            get; set;
        }

        // mapping
        public User User1
        {
            get; set;
        }

        public User User2
        {
            get; set;
        }
    }
}
