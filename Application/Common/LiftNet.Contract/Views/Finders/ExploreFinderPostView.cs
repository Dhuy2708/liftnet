using LiftNet.Contract.Enums.Appointment;
using LiftNet.Contract.Enums.Finder;
using LiftNet.Contract.Views.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Finders
{
    public class ExploreFinderPostView
    {
        public string Id
        {
            get; set;
        }
        public string Title
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public UserOverview? Poster
        {
            get; set;
        }

        public DateTime? StartTime
        {
            get; set;
        }

        public DateTime? EndTime
        {
            get; set;
        }

        public int StartPrice
        {
            get; set;
        }

        public int? EndPrice
        {
            get; set;
        }

        // address
        public double? Lat
        {
            get; set;
        }

        public double? Lng
        {
            get; set;
        }

        public string? PlaceName
        {
            get; set;
        }

        public float DistanceAway
        {
            get; set;
        }

        // hide option
        public bool IsAnonymous
        {
            get; set;
        }

        public bool HideAddress
        {
            get; set;
        }

        public FinderPostApplyingStatus ApplyingStatus
        {
            get; set;
        }

        public RepeatingType RepeatType
        {
            get; set;
        }

        public FinderPostStatus Status
        {
            get; set;
        }

        public DateTime CreatedAt
        {
            get; set;
        }
    }
}
