using LiftNet.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Engine.Data.Normalized
{

    public class NormalizedUserFeedFeature
    {
        public required NormalizedUserFieldAware User
        {
            get; set;
        }

        public required NormalizedFeedFieldAware Feed
        {
            get; set;
        }

        public float Label
        {
            get; set;
        }
    }

    public class NormalizedUserFieldAware
    {
        public string UserId
        {
            get; set;
        } = string.Empty;

        public float[] AgeRangeOneHot
        {
            get; set;
        } = Array.Empty<float>();

        public float[] RoleOneHot
        {
            get; set;
        } = Array.Empty<float>();

        public float[] GenderOneHot
        {
            get; set;
        } = Array.Empty<float>();

        public float NormalizedLat
        {
            get; set;
        }

        public float NormalizedLng
        {
            get; set;
        }
    }


    public class NormalizedFeedFieldAware
    {
        public string FeedId
        {
            get; set;
        } = string.Empty;

        public float NormalizedLikes
        {
            get; set;
        }

        public float NormalizedTimeAgo
        {
            get; set;
        }
    }
}
