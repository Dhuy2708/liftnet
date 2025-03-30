using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("SystemJobs")]
    public sealed class SystemJob
    {
        [Key]
        public required string Id
        {
            get; set;
        }

        public int Type
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
    }
}
