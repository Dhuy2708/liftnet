using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("UserPhysicalStats")]
    public sealed class UserPhysicalStat
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

        public int? Age
        {
            get; set;
        }

        public int? Gender
        {
            get; set;
        }

        public int? Height // cm
        {
            get; set;
        }

        public float? Mass // kg
        {
            get; set;
        }

        public float? Bdf
        {
            get; set;
        }

        public int? ActivityLevel
        {
            get; set;
        }   

        public int? Goal
        {
            get; set;
        }

        // mapping
        public User User
        {
            get; set;
        }
    }
}
