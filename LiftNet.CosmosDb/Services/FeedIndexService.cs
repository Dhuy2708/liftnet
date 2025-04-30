using LiftNet.Contract.Constants;
using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Enums;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.CosmosDb.Contracts;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Interfaces;
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
        
        public FeedIndexService(CosmosCredential cred, ILiftLogger<FeedIndexService> logger) 
            : base(cred, CosmosContainerId.Feed)
        {
            _logger = logger;
            _likeIndexService = new IndexBaseService<LikeIndexData>(cred, "feed");
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
                    ModifiedAt = DateTime.UtcNow
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

        public async Task<bool> LikeFeedAsync(string feedId, string userId)
        {
            try
            {
                // Check if feed exists
                var feed = await GetAsync(feedId, userId);
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

        public async Task<bool> UnlikeFeedAsync(string feedId, string userId)
        {
            try
            {
                // Check if feed exists
                var feed = await GetAsync(feedId, userId);
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
                
                var (likes, _) = await _likeIndexService.QueryAsync(condition);
                return likes.Count;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting like count for feed {feedId}");
                return 0;
            }
        }
    }
}
