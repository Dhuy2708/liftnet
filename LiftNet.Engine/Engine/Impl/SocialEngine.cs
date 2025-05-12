using LiftNet.Contract.Enums.Social;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using LiftNet.Engine.Contract;
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

        public async Task ComputeAllUserScores()
        {
            isBatchProcessing = true;
            try
            {
                var roleDict = await _roleService.GetAllRoleDictAsync();
                var adminRoleId = roleDict.FirstOrDefault(x => x.Value is LiftNetRoleEnum.Admin).Key;
                var queryable = _uow.UserRepo.GetQueryable();
                queryable = queryable.Include(u => u.Address);

                if (adminRoleId.IsNotNullOrEmpty())
                {
                    queryable = queryable.Where(x => !x.UserRoles.Any(x => x.RoleId.Eq(adminRoleId)));
                }
                var activeUsers = await queryable.Where(u => !u.IsDeleted && !u.IsSuspended)
                                                 .ToListAsync();

                var userIds = activeUsers.Select(u => u.Id).ToList();
                UserRoleDict = await _userService.GetUserIdRoleDict(userIds, roleDict);

                // Cache all social connections
                var connections = await _uow.SocialConnectionRepo.GetAll();
                FollowingCache = connections
                    .Where(c => c.Status == (int)SocialConnectionStatus.Following)
                    .GroupBy(c => c.UserId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(c => c.TargetId).ToList()
                    );

                for (int i = 0; i < activeUsers.Count; i += BATCH_SIZE)
                {
                    var batch = activeUsers.Skip(i).Take(BATCH_SIZE).ToList();
                    await ProcessUserBatch(batch, activeUsers);
                }
            }
            finally
            {
                isBatchProcessing = false;
                FollowingCache.Clear();
            }
        }

        public async Task ComputeUserScore(string userId)
        {
            isBatchProcessing = false;
            FollowingCache.Clear();

            var queryable = _uow.UserRepo.GetQueryable()
                                         .Include(x => x.Address);
            var user1 = await queryable.FirstOrDefaultAsync(x => x.Id == userId);
            if (user1 == null || user1.IsDeleted || user1.IsSuspended) return;

            var allUsers = await _uow.UserRepo.GetAll();
            var activeUsers = allUsers.Where(u => !u.IsDeleted && !u.IsSuspended && u.Id != userId).ToList();
            var userIds = activeUsers.Select(u => u.Id).ToList();
            UserRoleDict = await _userService.GetUserIdRoleDict(userIds);

            await ProcessUserBatch(new List<User> { user1 }, activeUsers);
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
                        // Only create new score if neither combination exists
                        newScores.Add(new SocialSimilarityScore
                        {
                            UserId1 = user1.Id,
                            UserId2 = user2.Id,
                            Score = score
                        });
                        // Add to existing keys to prevent duplicates within the same batch
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
