using LiftNet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Engine.Contract
{
    public class UserSimilarityFeature
    {
        public required string Id
        {
            get; set;
        }
        public int Age
        {
            get; set;
        }
        public LiftNetRoleEnum Role
        {
            get; set;
        }
        public List<string> FollowingIds
        {
            get; set;
        } = [];
        public double Lat
        {
            get; set;
        }
        public double Lng
        {
            get; set;
        }
    }
}
