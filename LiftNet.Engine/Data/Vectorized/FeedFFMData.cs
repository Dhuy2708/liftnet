using Microsoft.ML.Data;
using System;

namespace LiftNet.Engine.Data.Vectorized
{
    public class FeedFFMData
    {
        [VectorType(8)] // UserAgeRange has 8 possible values (0-7)
        public float[] UserAgeRangeOneHot { get; set; }

        [VectorType(3)] // LiftNetRoleEnum has 3 possible values (None, Seeker, Coach)
        public float[] UserRoleOneHot { get; set; }

        [VectorType(4)] // LiftNetGender has 3 possible values (None, Male, Female)
        public float[] UserGenderOneHot { get; set; }

        [VectorType(1)]
        public float[] LocationProximity { get; set; }

        [VectorType(1)]
        public float[] HasLocation { get; set; }

        [VectorType(1)]
        public float[] HasAge { get; set; }

        [VectorType(1)]
        public float[] HasGender { get; set; }

        [VectorType(1)]
        public float[] FeedLikes { get; set; }

        [VectorType(1)]
        public float[] FeedTimeAgo { get; set; }

        public bool Label { get; set; }
    }

    public class FeedFFMPrediction
    {
        public float Score { get; set; }
    }
}