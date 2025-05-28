using LiftNet.Contract.Enums.Finder;
using LiftNet.Contract.Views.Users;
using System;

namespace LiftNet.Contract.Views.Finders
{
    public class FinderPostApplicantView
    {
        public int Id 
        { 
            get; set; 
        }
        public string PostId 
        { 
            get; set; 
        }
        public UserOverview Trainer 
        { 
            get; set; 
        }
        public string? Message
        { 
            get; set; 
        }
        public string? CancelReason 
        { 
            get; set; 
        }
        public FinderPostApplyingStatus Status 
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
    }
} 