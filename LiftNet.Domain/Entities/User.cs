using LiftNet.Domain.Constants;
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
        public string FirstName
        {
            get; set;
        }

        [Required]
        public string LastName
        {
            get; set;
        }

        [DefaultValue(DomainConstants.DEFAULT_USER_AVATAR)]
        public string Avatar
        {
            get; set;
        }

        public string? Address
        {
            get; set;
        }

        // extension based on role
        public string? Extension
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

        public bool IsSuspended
        {
            get; set;
        }

        public ICollection<UserRole> UserRoles
        {
            get; set;
        }
    }
}
