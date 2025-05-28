using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("Addresses")]
    public sealed class Address
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id
        {
            get; set;
        } = Guid.NewGuid().ToString();

        public string PlaceName
        {
            get; set;
        }

        public string FormattedAddress
        {
            get; set;
        }

        public double Lat
        {
            get; set;
        }

        public double Lng
        {
            get; set;
        }

        public string PlaceId
        {
            get; set;
        }

        public DateTime CreatedAt
        {
            get; set;
        } = DateTime.UtcNow;

        public DateTime ModifiedAt
        {
            get; set;
        } = DateTime.UtcNow;

        // mapping
        public User User
        {
            get; set;
        }
    }
}
