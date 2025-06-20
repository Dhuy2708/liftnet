using Azure.Core;
using LiftNet.Contract.Constants;
using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Enums;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.CosmosDb.Contracts;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Interfaces;
using LiftNet.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.CosmosDb.Services
{
    public class FeedIndexService : IndexBaseService<FeedIndexData>, IFeedIndexService
    {
        private readonly ILiftLogger<FeedIndexService> _logger;
        private readonly IndexBaseService<LikeIndexData> _likeIndexService;
        private readonly IndexBaseService<CommentIndexData> _commentService;
        
        public FeedIndexService(CosmosCredential cred, ILiftLogger<FeedIndexService> logger) 
            : base(cred, CosmosContainerId.Feed)
        {
            _logger = logger;
            _likeIndexService = new IndexBaseService<LikeIndexData>(cred, "feed");
            _commentService = new IndexBaseService<CommentIndexData>(cred, "feed");
        }

        public async Task<FeedIndexData?> PostFeedAsync(string userId, string content, List<string> medias)
        {
            try
            {
                var feed = new FeedIndexData
                {
                    UserId = userId,
                    Content = content,
                    Medias = medias ?? new List<string>(),
                    Schema = DataSchema.Feed,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    Rand = (float)new Random().NextDouble()
                };

                return await UpsertAsync(feed);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error posting feed");
                return null;
            }
        }

        public async Task<FeedIndexData?> UpdateFeedAsync(string id, string userId, string? content = null, List<string>? medias = null)
        {
            try
            {
                var existingFeed = await GetAsync(id, userId);
                if (existingFeed == null || existingFeed.UserId != userId)
                    return null;

                // Only update fields that are provided
                if (content != null)
                {
                    existingFeed.Content = content;
                }

                if (medias != null)
                {
                    existingFeed.Medias = medias;
                }

                existingFeed.ModifiedAt = DateTime.UtcNow;
                existingFeed.Rand = (float)new Random().NextDouble();
                return await UpsertAsync(existingFeed);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error updating feed {id}");
                return null;
            }
        }

        public async Task<bool> DeleteFeedAsync(string id, string userId)
        {
            try
            {
                return await DeleteAsync(id, userId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error deleting feed {id}");
                return false;
            }
        }

        public async Task<bool> LikeFeedAsync(string feedId, string feedOwnerId, string userId)
        {
            try
            {
                // Check if feed exists
                var feed = await GetAsync(feedId, feedOwnerId);
                if (feed == null)
                    return false;

                // Check if already liked
                if (await HasUserLikedFeedAsync(feedId, userId))
                    return false;

                // Create like record
                var like = new LikeIndexData
                {
                    FeedId = feedId,
                    UserId = userId,
                    Schema = DataSchema.Like,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };

                await _likeIndexService.UpsertAsync(like);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error liking feed {feedId}");
                return false;
            }
        }

        public async Task<bool> UnlikeFeedAsync(string feedId, string feedOwnerId, string userId)
        {
            try
            {
                // Check if feed exists
                var feed = await GetAsync(feedId, feedOwnerId);
                if (feed == null)
                    return false;

                // Find like record
                var condition = new QueryCondition();
                condition.AddCondition(new ConditionItem("feedid", new List<string> { feedId }, FilterType.String));
                condition.AddCondition(new ConditionItem("userid", new List<string> { userId }, FilterType.String, QueryOperator.Equal, QueryLogic.And));
                
                var (likes, _) = await _likeIndexService.QueryAsync(condition);
                var like = likes.FirstOrDefault();
                
                if (like == null)
                    return false;

                // Delete like record
                return await _likeIndexService.DeleteAsync(like.Id, userId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error unliking feed {feedId}");
                return false;
            }
        }

        public async Task<bool> HasUserLikedFeedAsync(string feedId, string userId)
        {
            try
            {
                var condition = new QueryCondition();
                condition.AddCondition(new ConditionItem("feedid", new List<string> { feedId }, FilterType.String));
                condition.AddCondition(new ConditionItem("userid", new List<string> { userId }, FilterType.String, QueryOperator.Equal, QueryLogic.And));
                condition.AddCondition(new ConditionItem("schema", new List<string> { $"{(int)DataSchema.Like}" }, FilterType.Integer, QueryOperator.Equal, QueryLogic.And));
                
                return await _likeIndexService.AnyAsync(condition);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error checking like status for feed {feedId}");
                return false;
            }
        }

        public async Task<int> GetFeedLikeCountAsync(string feedId)
        {
            try
            {
                var condition = new QueryCondition();
                condition.AddCondition(new ConditionItem("feedid", new List<string> { feedId }, FilterType.String));
                condition.AddCondition(new ConditionItem("schema", new List<string> { $"{(int)DataSchema.Like}" }, FilterType.Integer, QueryOperator.Equal, QueryLogic.And));
                
                return await _likeIndexService.CountAsync(condition);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting like count for feed {feedId}");
                return 0;
            }
        }

        public async Task<Dictionary<string, int>> GetFeedLikeCountsAsync(List<string> feedIds)
        {
            try
            {
                if (feedIds.IsNullOrEmpty())
                {
                    return [];
                }
                var condition = new QueryCondition();
                condition.AddCondition(new ConditionItem("feedid", feedIds, FilterType.String));
                condition.AddCondition(new ConditionItem("schema", new List<string> { $"{(int)DataSchema.Like}" }, FilterType.Integer, QueryOperator.Equal, QueryLogic.And));
                
                var (likes, _) = await _likeIndexService.QueryAsync(condition);
                
                var counts = likes
                    .GroupBy(x => x.FeedId)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Ensure all requested feedIds are in the dictionary, even if they have 0 likes
                foreach (var feedId in feedIds)
                {
                    if (!counts.ContainsKey(feedId))
                    {
                        counts[feedId] = 0;
                    }
                }

                return counts;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting like counts for multiple feeds");
                return feedIds.ToDictionary(id => id, _ => 0);
            }
        }

        public async Task<Dictionary<string, int>> GetFeedCommentCountsAsync(List<string> feedIds)
        {
            try
            {
                if (feedIds.IsNullOrEmpty())
                {
                    return [];
                }
                var condition = new QueryCondition();
                condition.AddCondition(new ConditionItem("feedid", feedIds, FilterType.String));
                condition.AddCondition(new ConditionItem("schema", new List<string> { $"{(int)DataSchema.Comment}" }, FilterType.Integer, QueryOperator.Equal, QueryLogic.And));

                var (likes, _) = await _likeIndexService.QueryAsync(condition);

                var counts = likes
                    .GroupBy(x => x.FeedId)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Ensure all requested feedIds are in the dictionary, even if they have 0 likes
                foreach (var feedId in feedIds)
                {
                    if (!counts.ContainsKey(feedId))
                    {
                        counts[feedId] = 0;
                    }
                }

                return counts;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting like counts for multiple feeds");
                return feedIds.ToDictionary(id => id, _ => 0);
            }
        }

        public async Task<Dictionary<string, bool>> GetFeedLikeStatusesAsync(List<string> feedIds, string userId)
        {
            try
            {
                var condition = new QueryCondition();
                condition.AddCondition(new ConditionItem("feedid", feedIds, FilterType.String));
                condition.AddCondition(new ConditionItem("userid", new List<string> { userId }, FilterType.String, QueryOperator.Equal, QueryLogic.And));
                condition.AddCondition(new ConditionItem("schema", new List<string> { $"{(int)DataSchema.Like}" }, FilterType.Integer, QueryOperator.Equal, QueryLogic.And));
                
                var (likes, _) = await _likeIndexService.QueryAsync(condition);
                
                var likedFeedIds = likes.Select(x => x.FeedId).ToHashSet();
                
                return feedIds.ToDictionary(
                    id => id,
                    id => likedFeedIds.Contains(id)
                );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting like statuses for multiple feeds");
                return feedIds.ToDictionary(id => id, _ => false);
            }
        }

        public async Task<Dictionary<string, List<string>>> GetFeedLikesAsync(List<string> feedIds)
        {
            try
            {
                var condition = new QueryCondition();
                condition.AddCondition(new ConditionItem("feedid", feedIds, FilterType.String));
                condition.AddCondition(new ConditionItem("schema", new List<string> { $"{(int)DataSchema.Like}" }, FilterType.Integer, QueryOperator.Equal, QueryLogic.And));
                
                var (likes, _) = await _likeIndexService.QueryAsync(condition);
                
                return likes
                    .GroupBy(x => x.FeedId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.UserId).ToList()
                    );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting likes for feeds");
                return feedIds.ToDictionary(id => id, _ => new List<string>());
            }
        }

        public async Task<bool> UpdateAllFeedsRandomFieldAsync()
        {
            try
            {
                var condition = new QueryCondition();
                condition.PageSize = 0;
                condition.AddCondition(new ConditionItem("schema", new List<string> { $"{(int)DataSchema.Feed}" }, FilterType.Integer));
                
                var (feeds, _) = await QueryAsync(condition);
                
                foreach (var feed in feeds)
                {
                    feed.Rand = (float)new Random().NextDouble();
                    feed.ModifiedAt = DateTime.UtcNow;
                    await UpdateFeedAsync(feed.Id, feed.UserId);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating random fields for all feeds");
                return false;
            }
        }

        public async Task<bool> CommentFeedAsync(string feedId, string userId, string comment, string? parentId)
        {
            try
            {
                var condition = new QueryCondition();
                condition.AddCondition(new ConditionItem("id", feedId));
                condition.AddCondition(new ConditionItem(DataSchema.Feed, logic: QueryLogic.And));
                var feedResult = await QueryAsync(condition);
                if (!feedResult.Items.Any())
                {
                    return false;
                }

                var now = DateTime.UtcNow;

                var index = new CommentIndexData
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Schema = DataSchema.Comment,
                    CreatedAt = now,
                    ModifiedAt = now,
                    FeedId = feedId,
                    Comment = comment,
                    ParentId = parentId,
                    IsRoot = parentId.IsNullOrEmpty() ? true : false
                };
                await _commentService.UpsertAsync(index);
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Error commenting on feed {feedId}");
                return false;
            }
            return true;
        }
    }
}
