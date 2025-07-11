﻿using LiftNet.Contract.Enums.Social;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Enums;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Interfaces;
using LiftNet.Engine.Data.Feat;
using LiftNet.Engine.ML;
using LiftNet.Contract.Constants;
using LiftNet.Contract.Dtos.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiftNet.Utility.Extensions;
using LiftNet.Contract.Enums;
using LiftNet.Engine.Engine;
using System.Threading;
using LiftNet.Utility.Mappers;
using LiftNet.Domain.Entities;
using LiftNet.Redis.Interface;
using System.Collections.Concurrent;

namespace LiftNet.Service.Services
{
    internal class RecommendationService : IRecommendationService
    {
        private const int CACHE_EXPIRATION_DAYS = 1;
        private const int MAX_FOLLOWING_USERS = 5;
        private const int MAX_CONTENT_BASED_FEED_PER_USER = 25;
        private const int MAX_COLLABORATIVE_FEEDS_COUNT = 50;


        private readonly IUnitOfWork _uow;
        private readonly IUserService _userService;
        private readonly IFeedIndexService _feedIndexService;
        private readonly ISocialConnectionRepo _socialConnectionRepo;
        private readonly ISocialSimilarityScoreRepo _similarityScoreRepo;
        private readonly IRedisCacheService _redisCache;
        private readonly IFeedEngine _engine;
        private readonly IRoleService _roleService;
        private readonly ILiftLogger<RecommendationService> _logger;

        private Object _lock = new Object();

        public RecommendationService(IUnitOfWork uow,
                                     IUserService userService,
                                     IFeedIndexService feedIndexService,
                                     ISocialConnectionRepo socialConnectionRepo,
                                     ISocialSimilarityScoreRepo similarityScoreRepo,
                                     IRedisCacheService redisCache,
                                     IFeedEngine model,
                                     IRoleService roleService,
                                     ILiftLogger<RecommendationService> logger)
        {
            _uow = uow;
            _userService = userService;
            _feedIndexService = feedIndexService;
            _socialConnectionRepo = socialConnectionRepo;
            _similarityScoreRepo = similarityScoreRepo;
            _redisCache = redisCache;
            _engine = model;
            _roleService = roleService;
            _logger = logger;
        }

        #region feed
        public async Task<List<FeedIndexData>> ListRecommendedFeedsAsync(string userId, int pageSize = 5)
        {
            try
            {
                var seenFeedsKey = string.Format(RedisCacheKeys.SEEN_FEEDS_CACHE_KEY, userId);
                var seenFeeds = await _redisCache.GetObjectAsync<HashSet<string>>(seenFeedsKey) ?? new HashSet<string>();

                var contentBasedTask = GetContentBasedFeedsAsync(userId, seenFeeds);
                var collaborativeTask = GetCollaborativeFeedsAsync(userId, seenFeeds);

                await Task.WhenAll(contentBasedTask, collaborativeTask);
                var contentBasedFeeds = contentBasedTask.Result;
                var collaborativeFeeds = collaborativeTask.Result;

                var allFeeds = new Dictionary<string, (FeedIndexData Feed, float Score)>();
                foreach (var feed in contentBasedFeeds)
                {
                    allFeeds.TryAdd(feed.Feed.Id, (feed.Feed, feed.Score));
                }
                foreach (var feed in collaborativeFeeds)
                {
                    if (allFeeds.ContainsKey(feed.Feed.Id))
                    {
                        continue;
                    }
                    allFeeds.TryAdd(feed.Feed.Id, (feed.Feed, feed.Score));
                }
                var recommendedFeeds = allFeeds.Values
                            .OrderByDescending(x => x.Score)
                            .ThenByDescending(x => x.Feed.CreatedAt)
                            .Take(pageSize)
                            .Select(x => x.Feed)
                            .ToList();

                if (recommendedFeeds.IsNullOrEmpty())
                {
                    await _redisCache.RemoveAsync(seenFeedsKey);
                }
                else
                {
                    foreach (var feed in recommendedFeeds)
                    {
                        seenFeeds.Add(feed.Id);
                    }
                    await _redisCache.SetAsync(seenFeedsKey, seenFeeds, TimeSpan.FromDays(CACHE_EXPIRATION_DAYS));
                }
                    
                return recommendedFeeds;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while listing recommended feeds");
            }
            return [];
        }

        private async Task<List<(FeedIndexData Feed, float Score)>> GetContentBasedFeedsAsync(string userId, HashSet<string> seenFeeds)
        {
            var result = new List<(FeedIndexData Feed, float Score)>();
            var topFollowingUsers = await _socialConnectionRepo.GetQueryable()
                            .AsNoTracking()
                            .Where(x => x.UserId == userId && 
                                        x.Status == (int)SocialConnectionStatus.Following)
                            .OrderBy(x => Guid.NewGuid()) 
                            .Take(MAX_FOLLOWING_USERS)
                            .Select(x => x.TargetId)
                            .ToListAsync();
            if (topFollowingUsers.IsNullOrEmpty())
            {
                return [];
            }
            var condition = new QueryCondition();
            condition.PageSize = MAX_CONTENT_BASED_FEED_PER_USER;
            condition.AddCondition(new ConditionItem("userid", topFollowingUsers, logic: QueryLogic.And));
            if (seenFeeds.IsNotNullOrEmpty())
            {
                condition.AddCondition(new ConditionItem("id", seenFeeds.ToList(), FilterType.String, QueryOperator.NotContains, logic: QueryLogic.And));
            }
            condition.AddCondition(new ConditionItem("schema", [$"{(int)DataSchema.Feed}"], FilterType.Integer, logic: QueryLogic.And));
            var (feeds, _) = await _feedIndexService.QueryAsync(condition);

            var userScores = await _similarityScoreRepo.GetQueryable()
                                    .AsNoTracking()
                                    .Where(x => x.UserId1 == userId &&
                                                topFollowingUsers.Contains(x.UserId2))
                                    .Select(x => new { x.UserId2, x.Score })
                                    .ToListAsync();
            foreach (var followingUserId in topFollowingUsers)
            {
                var userFeeds = feeds
                    .Where(f => string.Equals(f.UserId, followingUserId))
                    .OrderByDescending(f => f.CreatedAt)
                    .ToList();
                var userScore = userScores.FirstOrDefault(x => x.UserId2 == followingUserId);
                foreach (var feed in userFeeds)
                {
                    var score = userScore?.Score ?? 0.1f;
                    result.Add((feed, score));
                }
            }
            return result;
        }

        private async Task<List<(FeedIndexData Feed, float Score)>> GetCollaborativeFeedsAsync(string userId, HashSet<string> seenFeeds)
        {
            var threshold = (float)new Random().NextDouble();
            var condition = new QueryCondition();
            condition.PageSize = MAX_COLLABORATIVE_FEEDS_COUNT;
            condition.AddCondition(new ConditionItem("schema", [$"{(int)DataSchema.Feed}"], FilterType.Integer, logic: QueryLogic.And));
            condition.AddCondition(new ConditionItem("userid", [userId], queryOperator: QueryOperator.NotEqual, logic: QueryLogic.And));
            if (seenFeeds.IsNotNullOrEmpty())
            {
                condition.AddCondition(new ConditionItem("id", seenFeeds.ToList(), FilterType.String, QueryOperator.NotContains, logic: QueryLogic.And));
            }
            condition.AddCondition(new ConditionItem("rand", [threshold.ToString()], FilterType.Float, QueryOperator.GreaterThanOrEqual, logic: QueryLogic.And));
            var (feeds, _) = await _feedIndexService.QueryAsync(condition);

            if (feeds.Count < MAX_COLLABORATIVE_FEEDS_COUNT)
            {
                condition.PageSize = MAX_COLLABORATIVE_FEEDS_COUNT - feeds.Count;
                condition.UpdateCondition(new ConditionItem("rand", [threshold.ToString()], FilterType.Float, QueryOperator.LessThan, logic: QueryLogic.And));
                var (extraResult, _) = await _feedIndexService.QueryAsync(condition);
                feeds.AddRange(extraResult);
            }

            var result = new ConcurrentBag<(FeedIndexData Feed, float Score)>();
            var tasks = feeds.Select(feed => Task.Run(() =>
            {
                try
                {
                    var feature = new UserFeedFeature
                    {
                        User = new UserFieldAware { UserId = userId },
                        Feed = new FeedFieldAware { FeedId = feed.Id }
                    };
                    var score = _engine.Predict(feature);
                    score = 1 / (1 + MathF.Exp(-score));
                    result.Add((feed, score));
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Error predicting feed {feed.Id} for user {userId}");
                }
            })).ToList();
            await Task.WhenAll(tasks);
            return result.ToList();
        }
        #endregion

        #region search
        public async Task<List<UserOverview>> SearchPrioritizedUser(string currentUserId, string search, int pageSize = 10, int pageNumber = 1)
        {
            if (string.IsNullOrEmpty(currentUserId))
                throw new ArgumentException("Current user ID cannot be null or empty", nameof(currentUserId));

            if (pageSize <= 0)
                pageSize = 10;
            if (pageNumber <= 0)
                pageNumber = 1;

            var query = _uow.UserRepo.GetQueryable()
                .Include(u => u.UserRoles)
                .Where(u => !u.IsDeleted && !u.IsSuspended && u.Id != currentUserId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(u =>
                    (u.UserName != null && u.UserName.ToLower().Contains(search)) ||
                    (u.Email != null && u.Email.ToLower().Contains(search)) ||
                    (u.FirstName != null && u.FirstName.ToLower().Contains(search)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(search)) ||
                    ((u.FirstName + " " + u.LastName).ToLower().Contains(search))
                );
            }

            // Get similarity scores for the current user
            var similarityScores = await _uow.SocialSimilarityScoreRepo.GetQueryable()
                .Where(s => s.UserId1 == currentUserId)
                .ToListAsync();


            var similarityScoresDict = similarityScores.ToDictionary(s => s.UserId2, s => s.Score);

            var users = await query
                .AsNoTracking()
                .ToListAsync();

            var usersWithScores = users
                                .Select(u => new
                                {
                                    User = u,
                                    Score = similarityScoresDict.TryGetValue(u.Id, out var score) ? score : 0
                                })
                                .OrderByDescending(x => x.Score)
                                .ToList();

            users = usersWithScores
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.User)
                .ToList();

            var userIds = users.Select(x => x.Id).ToList();

            var followingStatus = await _uow.SocialConnectionRepo.GetQueryable()
                .Where(c => c.UserId == currentUserId &&
                            userIds.Contains(c.TargetId) &&
                            c.Status == (int)SocialConnectionStatus.Following)
                .Select(c => c.TargetId)
                .ToListAsync();

            var roleDict = await _userService.GetUserIdRoleDict(userIds);

            return users.Select(x => new UserOverview
            {
                Id = x.Id,
                Email = x.Email ?? string.Empty,
                Username = x.UserName ?? string.Empty,
                FirstName = x.FirstName ?? string.Empty,
                LastName = x.LastName ?? string.Empty,
                Role = roleDict.GetValueOrDefault(x.Id, LiftNetRoleEnum.None),
                Avatar = x.Avatar ?? string.Empty,
                IsDeleted = x.IsDeleted,
                IsSuspended = x.IsSuspended,
                IsFollowing = followingStatus.Contains(x.Id)
            }).ToList();
        }
        #endregion

        #region friend suggestion
        public async Task<List<UserOverview>> SuggestFriendsAsync(string userId, int pageSize = 10)
        {
            var suggestedUserKey = string.Format(RedisCacheKeys.SUGGESTED_FRIENDS_CACHE_KEY, userId);
            var suggestedUserIds = await _redisCache.GetObjectAsync<HashSet<string>>(suggestedUserKey) ?? new HashSet<string>();

            var followingIds = await _socialConnectionRepo.GetQueryable()
                .Where(x => x.UserId == userId && x.Status == (int)SocialConnectionStatus.Following)
                .Select(x => x.TargetId)
                .ToListAsync();

            var removeUserIds = followingIds.Concat(suggestedUserIds).Distinct().ToHashSet();
            var thresholds = new List<double> { 0.4, 0.2, 0.0 };
            var collected = new List<SocialSimilarityScore>();

            foreach (var threshold in thresholds)
            {
                var candidates = await _similarityScoreRepo.GetQueryable()
                    .Where(x => x.UserId1 == userId &&
                                x.Score > threshold &&
                                !removeUserIds.Contains(x.UserId2))
                    .OrderBy(x => Guid.NewGuid())
                    .Select(x => new SocialSimilarityScore
                    {
                        UserId2 = x.UserId2,
                        Score = x.Score
                    })
                    .Take(100)
                    .ToListAsync();

                collected.AddRange(candidates);
                removeUserIds.UnionWith(candidates.Select(x => x.UserId2));

                if (collected.Count >= pageSize)
                    break;
            }

            // query more
            if (collected.Count < pageSize)
            {
                await _redisCache.RemoveAsync(suggestedUserKey);
                suggestedUserIds.Clear();
                removeUserIds = followingIds.ToHashSet();

                collected.Clear();
                foreach (var threshold in thresholds)
                {
                    var candidates = await _similarityScoreRepo.GetQueryable()
                        .Where(x => x.UserId1 == userId &&
                                    x.Score > threshold &&
                                    !removeUserIds.Contains(x.UserId2))
                        .OrderBy(x => Guid.NewGuid())
                        .Select(x => new SocialSimilarityScore
                        {
                            UserId2 = x.UserId2,
                            Score = x.Score
                        })
                        .Take(100)
                        .ToListAsync();

                    collected.AddRange(candidates);
                    removeUserIds.UnionWith(candidates.Select(x => x.UserId2));

                    if (collected.Count >= pageSize)
                        break;
                }
            }

            var userIds = collected
                .OrderByDescending(x => x.Score)
                .Take(pageSize)
                .Select(x => x.UserId2)
                .ToList();

            var suggestUsers = await _userService.GetByIds(userIds);
            var roleDict = await _userService.GetUserIdRoleDict(userIds);
            var result = suggestUsers.Select(x => UserMapper.ToOverview(x, roleDict)).ToList();

            foreach (var id in userIds)
                suggestedUserIds.Add(id);

            await _redisCache.SetAsync(suggestedUserKey, suggestedUserIds, TimeSpan.FromMinutes(5));
            return result;
        }
        #endregion
    }
}
