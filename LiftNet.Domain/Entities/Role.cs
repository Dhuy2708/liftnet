using LiftNet.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiftNet.Domain.Entities
{
    [Table("Roles")]
    public sealed class Role : IdentityRole, IEntity
    {
        public Role() : base()
        {
            
        }
        public Role(string roleName) : base(roleName)
        {
        }
    }
}
