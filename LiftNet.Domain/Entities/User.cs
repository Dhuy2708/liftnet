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

        public string? Location
        {
            get; set;
        }

        public int Age
        {
            get; set;
        }

        public int Gender
        {
            get; set;
        }

        [ForeignKey(nameof(Province))]
        public int? ProvinceCode
        {
            get; set;
        }

        [ForeignKey(nameof(District))]
        public int? DistrictCode
        {
            get; set;
        }

        [ForeignKey(nameof(Ward))]
        public int? WardCode
        {
            get; set;
        }

        [ForeignKey(nameof(Address))]
        public string? AddressId
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

        // mapping
        public Address? Address
        {
            get; set;
        }

        public Province? Province
        {
            get; set;
        }

        public District? District
        {
            get; set;
        }

        public Ward? Ward
        {
            get; set;
        }

        public ICollection<UserRole> UserRoles
        {
            get; set;
        }

        public CoachExtension? Extension
        {
            get; set;
        }
    }
}
