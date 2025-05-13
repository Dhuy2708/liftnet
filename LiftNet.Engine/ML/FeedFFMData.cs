using Microsoft.ML.Data;
using System;

namespace LiftNet.Engine.ML
{
    public class FeedFFMData
    {
        [VectorType]
        public float[] UserAgeRangeOneHot { get; set; }

        [VectorType]
        public float[] UserRoleOneHot { get; set; }

        [VectorType]
        public float[] UserGenderOneHot { get; set; }

        public float LocationProximity { get; set; }
        public float HasLocation { get; set; }
        public float HasAge { get; set; }
        public float HasGender { get; set; }
        public float FeedLikes { get; set; }
        public float FeedTimeAgo { get; set; }
        public float Label { get; set; }
    }

    public class FeedFFMPrediction
    {
        public float Score { get; set; }
    }
} 