using LiftNet.Contract.Constants;
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
        
        public FeedIndexService(CosmosCredential cred, ILiftLogger<FeedIndexService> logger) 
            : base(cred, CosmosContainerId.Feed)
        {
            _logger = logger;
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
                    Likes = 0,
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
    }
}
