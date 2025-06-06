using LiftNet.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("SocialConnections")]
    public sealed class SocialConnection : IEntity
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

        [ForeignKey(nameof(Target))]
        public string TargetId
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

        public User Target
        {
            get; set;
        }
    }
}
