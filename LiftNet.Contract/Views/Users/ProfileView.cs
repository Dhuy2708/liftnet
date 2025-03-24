using LiftNet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Users
{
    public class ProfileView
    {
        public string Id 
        {
            get; set;
        }
        public bool IsSelf
        {
            get; set;
        }
        public string UserName 
        { 
            get; set;
        }
        public string Email 
        { 
            get; set; 
        }
        public bool IsFollowing
        {
            get; set;
        }
        public int Following
        {
            get; set;
        }
        public int Follower
        {
            get; set;
        }
        public LiftNetRoleEnum Role
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
        public string Avatar 
        { 
            get; set; 
        }
    }
}
