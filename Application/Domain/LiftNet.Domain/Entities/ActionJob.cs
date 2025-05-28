using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("ActionJobs")]
    public sealed class ActionJob
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public int Type
        {
            get; set;
        }

        [ForeignKey(nameof(User))]
        public string UserId
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

        // mapping
        public User User
        {
            get; set;
        }
    }
}
