using LiftNet.Contract.Views.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Finders
{
    public class SeekerRecommendationView
    {
        public UserOverview Seeker
        {
            get; set;
        }

        public DateTimeOffset RecommendedAt
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }
    }
}
