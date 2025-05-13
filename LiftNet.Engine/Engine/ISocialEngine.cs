using LiftNet.Domain.Entities;
using LiftNet.Engine.Data.Feat;
using LiftNet.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Engine.Engine
{
    public interface ISocialEngine
    {
        List<SocialSimilarityScore> ComputeUserScores(List<UserSimilarityFeature> sourceUsers, List<UserSimilarityFeature> targetUsers);
        List<SocialSimilarityScore> UpdateExistingScores(List<SocialSimilarityScore> existingScores, List<UserSimilarityFeature> sourceUsers, List<UserSimilarityFeature> targetUsers);
    }
}
