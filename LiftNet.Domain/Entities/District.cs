using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("Districts")]
    public sealed class District
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public required int Code
        {
            get; set;
        }
        public string Codename
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

        [ForeignKey(nameof(Province))]
        public int ProvinceCode
        {
            get; set;
        }

        // mapping
        public Province Province
        {
            get; set;
        }

        public List<Ward> Wards
        {
            get; set;
        }
    }
}
