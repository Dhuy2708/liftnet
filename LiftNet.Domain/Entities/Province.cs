using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("Provinces")]
    public sealed class Province
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
        public int PhoneCode
        {
            get; set;
        }

        // mapping
        public List<District> Districts
        {
            get; set;
        }
    }
}
