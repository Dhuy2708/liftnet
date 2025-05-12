using LiftNet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Users
{
    public class UserOverview
    {
        public string Id
        {
            get; set;
        }
        public string Email
        {
            get; set;
        }
        public string Username
        {
            get; set;
        }
        public string FirstName
        {
            get; set;
        }
        public string LastName
        {
            get; set;
        }
        public LiftNetRoleEnum Role
        {
            get; set;
        }
        public string Avatar
        {
            get; set;
        }
        public bool IsDeleted
        {
            get; set;
        }
        public bool IsSuspended
        {
            get; set;
        }
        public bool IsFollowing
        {
            get; set;
        }
    }
}
