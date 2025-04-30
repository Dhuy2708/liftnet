using LiftNet.Domain.Indexes;
using LiftNet.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IServices.Indexes
{
    public interface IFeedIndexService : IIndexBaseService<FeedIndexData>, IDependency
    {
        Task<FeedIndexData?> PostFeedAsync(string userId, string content, List<string> medias);
        Task<FeedIndexData?> UpdateFeedAsync(string id, string userId, string? content = null, List<string>? medias = null);
        Task<bool> DeleteFeedAsync(string id, string userId);
        Task<bool> LikeFeedAsync(string feedId, string userId);
        Task<bool> UnlikeFeedAsync(string feedId, string userId);
        Task<bool> HasUserLikedFeedAsync(string feedId, string userId);
        Task<int> GetFeedLikeCountAsync(string feedId);
    }
}
