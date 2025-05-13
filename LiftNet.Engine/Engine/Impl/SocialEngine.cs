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
        private readonly IUnitOfWork _uow;
        private readonly UserSimilarityModel _similarityModel;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        private const int BATCH_SIZE = 100;
        private Dictionary<string, LiftNetRoleEnum> UserRoleDict = [];
        private Dictionary<string, List<string>> FollowingCache = [];
        private bool isBatchProcessing = false;

        public SocialEngine(IUnitOfWork uow, IUserService userService, IRoleService roleService)
        {
            _uow = uow;
            _userService = userService;
            _roleService = roleService;
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

        #region private
        private async Task ProcessUserBatch(List<User> sourceUsers, List<User> targetUsers)
        {
            var newScores = new List<SocialSimilarityScore>();
            var existingScores = await _uow.SocialSimilarityScoreRepo.GetAll();
            
            // Create a set of existing score keys for faster lookup
            var existingScoreKeys = new HashSet<string>();
            var scoresToUpdate = new List<SocialSimilarityScore>();
            
            foreach (var score in existingScores)
            {
                var key1 = $"{score.UserId1}_{score.UserId2}";
                var key2 = $"{score.UserId2}_{score.UserId1}";
                existingScoreKeys.Add(key1);
                existingScoreKeys.Add(key2);
            }

            foreach (var user1 in sourceUsers)
            {
                var user1Feature = await CreateUserFeature(user1);

                foreach (var user2 in targetUsers.Where(u => u.Id != user1.Id))
                {
                    var user2Feature = await CreateUserFeature(user2);
                    var score = _similarityModel.ComputeScore(user1Feature, user2Feature);

                    var key1 = $"{user1.Id}_{user2.Id}";
                    var key2 = $"{user2.Id}_{user1.Id}";

                    if (existingScoreKeys.Contains(key1) || existingScoreKeys.Contains(key2))
                    {
                        var existingScore = existingScores.FirstOrDefault(s => 
                            (s.UserId1 == user1.Id && s.UserId2 == user2.Id) || 
                            (s.UserId1 == user2.Id && s.UserId2 == user1.Id));

                        if (existingScore != null)
                        {
                            existingScore.Score = score;
                            scoresToUpdate.Add(existingScore);
                        }
                    }
                    else
                    {
                        newScores.Add(new SocialSimilarityScore
                        {
                            UserId1 = user1.Id,
                            UserId2 = user2.Id,
                            Score = score
                        });
                        existingScoreKeys.Add(key1);
                        existingScoreKeys.Add(key2);
                    }
                }
            }

            if (newScores.Any())
            {
                await _uow.SocialSimilarityScoreRepo.CreateRange(newScores);
            }

            if (scoresToUpdate.Any())
            {
                await _uow.SocialSimilarityScoreRepo.UpdateRange(scoresToUpdate);
            }

            await _uow.CommitAsync();
        }

        private async Task<UserSimilarityFeature> CreateUserFeature(User user)
        {
            List<string> followingIds;
            if (isBatchProcessing)
            {
                followingIds = FollowingCache.GetValueOrDefault(user.Id, []);
            }
            else
            {
                followingIds = await GetFollowingIds(user.Id);
            }

            var (lat, lng) = GetUserLocation(user);

            return new UserSimilarityFeature
            {
                Id = user.Id,
                Age = CalculateAge(user.CreatedAt),
                Role = UserRoleDict.GetValueOrDefault(user.Id, LiftNetRoleEnum.None),
                FollowingIds = followingIds,
                Lat = lat,
                Lng = lng
            };
        }

        private async Task<List<string>> GetFollowingIds(string userId)
        {
            var connections = await _uow.SocialConnectionRepo.GetAll();
            return connections
                .Where(c => c.UserId == userId && c.Status == (int)SocialConnectionStatus.Following)
                .Select(c => c.TargetId)
                .ToList();
        }

        private (double lat, double lng) GetUserLocation(User user)
        {
            if (user.Address != null)
            {
                return (user.Address.Lat, user.Address.Lng);
            }
            return (0, 0);
        }

        private int CalculateAge(DateTime createdAt)
        {
            return DateTime.UtcNow.Year - createdAt.Year;
        }
        #endregion
    }
}
