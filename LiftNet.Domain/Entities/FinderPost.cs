using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("FinderPosts")]
    public sealed class FinderPost
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id
        {
            get; set;
        }

        [ForeignKey(nameof(UserId))]
        public string UserId
        {
            get; set;
        }

        public string Title
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public DateTime? StartTime
        {
            get; set;
        }

        public DateTime? EndTime
        {
            get; set;
        }

        public int StartPrice
        {
            get; set;
        }

        public int? EndPrice
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

        public bool HideAddress
        {
            get; set;
        }

        public int RepeatType
        {
            get; set;
        }

        public int Status
        {
            get; set;
        }

        public DateTime CreatedAt
        {
            get; set;
        }

        public DateTime ModifiedAt
        {
            get; set;
        } = DateTime.UtcNow;

        // mapping
        public User User
        {
            get; set;
        }

        public List<FinderPostApplicant> Applicants
        {
            get; set;
        }
    }
}
