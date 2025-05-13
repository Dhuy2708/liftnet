using LiftNet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Engine.Data.Feat
{
    public class UserFeedFeature
    {
        public required UserFieldAware User
        {
            get; set;
        }

        public required FeedFieldAware Feed
        {
            get; set;
        }

        public float Label
        {
            get; set;
        }
    }

    public class UserFieldAware
    {
        public string UserId
        {
            get; set;
        } = string.Empty;

        public int Age
        {
            get; set;
        }

        public LiftNetRoleEnum Role
        {
            get; set;
        }

        public LiftNetGender Gender
        {
            get; set;
        }

        public (double Lat, double Lng)? Location
        {
            get; set;
        }
    }

    public class FeedFieldAware
    {
        public string FeedId
        {
            get; set;
        } = string.Empty;

        public int Likes
        {
            get; set;
        }

        public DateTime CreatedAt
        {
            get; set;
        }
    }
}
