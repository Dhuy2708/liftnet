using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("FinderPostApplicants")]
    public sealed class FinderPostApplicant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get; set;
        }

        [ForeignKey(nameof(Post))]
        public string PostId
        {
            get; set;
        }

        [ForeignKey(nameof(Trainer))]
        public string TrainerId
        {
            get; set;
        }

        public string Message
        {
            get; set;
        }

        public string CancelReason
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
        }

        public int Status
        {
            get; set;
        }

        // mapping
        public User Trainer
        {
            get; set;
        }

        public FinderPost Post
        {
            get; set;
        }
    }
}
