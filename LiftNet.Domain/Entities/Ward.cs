using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("Wards")]
    public sealed class Ward
    {
        [Key]
        public required int Code
        {
            get; set;
        }
        public string CodeName
        {
            get; set;
        }
        public string DivisionType
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }

        [ForeignKey(nameof(District))]
        public int DistrictCode
        {
            get; set;
        }

        // mapping
        public District District
        {
            get; set;
        }
    }
}
