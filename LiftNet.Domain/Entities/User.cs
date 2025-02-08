using LiftNet.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{

    [Table("Users")]
    public sealed class User : IdentityUser, IEntity
    {
        [Required]
        public required string Name
        {
            get; set;
        }

        [DefaultValue("https://res.cloudinary.com/dvwgt4tm1/image/upload/v1730031850/360_F_549983970_bRCkYfk0P6PP5fKbMhZMIb07mCJ6esXL_t9czwt.jpg")]
        public string? Avatar
        {
            get; set;
        }

        [Required]
        [DefaultValue("GETUTCDATE()")]
        public DateTime CreatedAt
        {
            get; set;
        } = DateTime.UtcNow;


        public bool IsDeleted
        {
            get; set;
        }
    }
}
