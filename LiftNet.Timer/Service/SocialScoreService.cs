using LiftNet.Contract.Enums.Job;
using LiftNet.Contract.Enums.Social;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using LiftNet.Domain.Interfaces;
using LiftNet.Engine.Data.Feat;
using LiftNet.Engine.Engine;
using LiftNet.Timer.Service.Common;
using LiftNet.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiftNet.Timer.Service
{
    public class SocialScoreService : BaseSystemJob
    {
        private ILiftLogger<SocialScoreService> _logger => _provider.GetRequiredService<ILiftLogger<SocialScoreService>>();
        private ISocialEngine _socialEngine => _provider.GetRequiredService<ISocialEngine>();
        private IUnitOfWork _uow => _provider.GetRequiredService<IUnitOfWork>();
        private IUserService _userService => _provider.GetRequiredService<IUserService>();
        private IRoleService _roleService => _provider.GetRequiredService<IRoleService>();

        private const int BATCH_SIZE = 100;

        public SocialScoreService(IServiceProvider provider) : base(JobType.AllSocialScoreUp, provider)
        {
        }

        protected override async Task<JobStatus> KickOffJobServiceAsync()
        {
            try
            {
                _logger.Info("begin to compute all social scores");
                await ComputeAllUserScores();
                return JobStatus.Finished;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while computing social scores");
                return JobStatus.Failed;
            }
        }

        private async Task ComputeAllUserScores()
        {
            var roleDict = await _roleService.GetAllRoleDictAsync();
            var adminRoleId = roleDict.FirstOrDefault(x => x.Value is LiftNetRoleEnum.Admin).Key;
            var queryable = _uow.UserRepo.GetQueryable();
            queryable = queryable.Include(u => u.Address);

            if (!string.IsNullOrEmpty(adminRoleId))
            {
                queryable = queryable.Where(x => !x.UserRoles.Any(x => x.RoleId == adminRoleId));
            }

            var activeUsers = await queryable.Where(u => !u.IsDeleted && !u.IsSuspended)
                                             .ToListAsync();

            var userIds = activeUsers.Select(u => u.Id).ToList();
            var userRoleDict = await _userService.GetUserIdRoleDict(userIds, roleDict);

            // Cache all social connections
            var connections = await _uow.SocialConnectionRepo.GetAll();
            var followingCache = connections
                .Where(c => c.Status == (int)SocialConnectionStatus.Following)
                .GroupBy(c => c.UserId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(c => c.TargetId).ToList()
                );

            var existingScores = (await _uow.SocialSimilarityScoreRepo.GetAll()).ToList();

            for (int i = 0; i < activeUsers.Count; i += BATCH_SIZE)
            {
                var batch = activeUsers.Skip(i).Take(BATCH_SIZE).ToList();
                await ProcessUserBatch(batch, activeUsers, userRoleDict, followingCache, existingScores);
            }
        }

        private async Task ProcessUserBatch(
            List<User> sourceUsers,
            List<User> targetUsers,
            Dictionary<string, LiftNetRoleEnum> userRoleDict,
            Dictionary<string, List<string>> followingCache,
            List<SocialSimilarityScore> existingScores)
        {
            var sourceFeatures = sourceUsers.Select(u => CreateUserFeature(u, userRoleDict, followingCache)).ToList();
            var targetFeatures = targetUsers.Select(u => CreateUserFeature(u, userRoleDict, followingCache)).ToList();

            var newScores = new List<SocialSimilarityScore>();
            var scoresToUpdate = new List<SocialSimilarityScore>();
            var existingScoreKeys = existingScores.Select(s => $"{s.UserId1}_{s.UserId2}").ToHashSet();

            foreach (var user1 in sourceFeatures)
            {
                foreach (var user2 in targetFeatures.Where(u => u.Id != user1.Id))
                {
                    var key = $"{user1.Id}_{user2.Id}";
                    var reverseKey = $"{user2.Id}_{user1.Id}";
                    var score = _socialEngine.ComputeUserScores(new List<UserSimilarityFeature> { user1 }, new List<UserSimilarityFeature> { user2 })[0];

                    if (existingScoreKeys.Contains(key) || existingScoreKeys.Contains(reverseKey))
                    {
                        var existingScore = existingScores.FirstOrDefault(s => s.UserId1 == user1.Id && s.UserId2 == user2.Id);
                        existingScore.Score = score.Score;
                        scoresToUpdate.Add(existingScore);
                    }
                    else
                    {
                        newScores.Add(score);
                    }
                }
            }

            if (scoresToUpdate.Any())
            {
                await _uow.SocialSimilarityScoreRepo.UpdateRange(scoresToUpdate);
            }

            if (newScores.Any())
            {
                await _uow.SocialSimilarityScoreRepo.CreateRange(newScores);
            }

            await _uow.CommitAsync();
        }

        private UserSimilarityFeature CreateUserFeature(
                            User user,
                            Dictionary<string, LiftNetRoleEnum> userRoleDict,
                            Dictionary<string, List<string>> followingCache)
        {
            var followingIds = followingCache.GetValueOrDefault(user.Id, new List<string>());
            var (lat, lng) = GetUserLocation(user);

            return new UserSimilarityFeature
            {
                Id = user.Id,
                Age = user.Age,
                Gender = (LiftNetGender)user.Gender,
                Role = userRoleDict.GetValueOrDefault(user.Id, LiftNetRoleEnum.None),
                FollowingIds = followingIds,
                Lat = lat,
                Lng = lng
            };
        }

        private (double lat, double lng) GetUserLocation(User user)
        {
            if (user.Address != null)
            {
                return (user.Address.Lat, user.Address.Lng);
            }
            return (0, 0);
        }
    }
}
