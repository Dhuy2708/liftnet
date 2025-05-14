using LiftNet.Contract.Enums.Social;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using LiftNet.Engine.Data.Feat;
using LiftNet.Engine.ML;
using LiftNet.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Engine.Engine.Impl
{
    public class SocialEngine : ISocialEngine
    {
        private readonly UserSimilarityModel _similarityModel;

        private const int BATCH_SIZE = 100;
        private Dictionary<string, LiftNetRoleEnum> UserRoleDict = [];
        private Dictionary<string, List<string>> FollowingCache = [];
        private bool isBatchProcessing = false;

        public SocialEngine()
        {
            _similarityModel = new UserSimilarityModel();
        }

        public List<SocialSimilarityScore> ComputeUserScores(
                        List<UserSimilarityFeature> sourceUsers,
                        List<UserSimilarityFeature> targetUsers)
        {
            var scores = new List<SocialSimilarityScore>();

            foreach (var user1 in sourceUsers)
            {
                foreach (var user2 in targetUsers.Where(u => u.Id != user1.Id))
                {
                    var score = _similarityModel.ComputeScore(user1, user2);
                    scores.Add(new SocialSimilarityScore
                    {
                        UserId1 = user1.Id,
                        UserId2 = user2.Id,
                        Score = score
                    });
                }
            }

            return scores;
        }

        public List<SocialSimilarityScore> UpdateExistingScores(List<SocialSimilarityScore> existingScores,
                                                                List<UserSimilarityFeature> sourceUsers,
                                                                List<UserSimilarityFeature> targetUsers)
        {
            var scoresToUpdate = new List<SocialSimilarityScore>();
            var existingScoreKeys = new HashSet<string>();

            foreach (var score in existingScores)
            {
                var key1 = $"{score.UserId1}_{score.UserId2}";
                var key2 = $"{score.UserId2}_{score.UserId1}";
                existingScoreKeys.Add(key1);
                existingScoreKeys.Add(key2);
            }

            foreach (var user1 in sourceUsers)
            {
                foreach (var user2 in targetUsers.Where(u => u.Id != user1.Id))
                {
                    var key1 = $"{user1.Id}_{user2.Id}";
                    var key2 = $"{user2.Id}_{user1.Id}";

                    if (existingScoreKeys.Contains(key1) || existingScoreKeys.Contains(key2))
                    {
                        var existingScore = existingScores.FirstOrDefault(s => 
                            (s.UserId1 == user1.Id && s.UserId2 == user2.Id) || 
                            (s.UserId1 == user2.Id && s.UserId2 == user1.Id));

                        if (existingScore != null)
                        {
                            existingScore.Score = _similarityModel.ComputeScore(user1, user2);
                            scoresToUpdate.Add(existingScore);
                        }
                    }
                }
            }

            return scoresToUpdate;
        }
    }
}
