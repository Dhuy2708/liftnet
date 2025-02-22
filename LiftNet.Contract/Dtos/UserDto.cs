using LiftNet.Domain.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Dtos
{
    public class UserDto
    {
        public string FirstName
        {
            get; set;
        }

        public string LastName
        {
            get; set;
        }

        public string Avatar
        {
            get; set;
        }

        public AddressDto Address
        {
            get; set;
        }

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
    }
}
