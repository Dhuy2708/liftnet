using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using MediatR;
using LiftNet.Domain.Response;
using LiftNet.Domain.ViewModels;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Handler.Feeds.Queries.Requests;
using System.Collections.Generic;
using LiftNet.Utility.Extensions;

namespace LiftNet.Handler.Feeds.Queries
{
    public class RecommendFeedsHandler : IRequestHandler<RecommendFeedsQuery, LiftNetRes<FeedViewModel>>
    {
        private readonly IRecommendationService _recommendationService;
        private readonly IFeedIndexService _feedIndexService;
        public RecommendFeedsHandler(IRecommendationService recommendationService, IFeedIndexService feedIndexService)
        {
            _recommendationService = recommendationService;
            _feedIndexService = feedIndexService;
        }

        public async Task<LiftNetRes<FeedViewModel>> Handle(RecommendFeedsQuery request, CancellationToken cancellationToken)
        {
            var feeds = await _recommendationService.ListRecommendedFeedsAsync(request.UserId, 5);
            var feedIds = feeds.Select(f => f.Id).ToList();
            if (feedIds.IsNullOrEmpty())
            {
                return LiftNetRes<FeedViewModel>.SuccessResponse([]);
            }
            var likeCounts = await _feedIndexService.GetFeedLikeCountsAsync(feedIds);
            var likeStatuses = await _feedIndexService.GetFeedLikeStatusesAsync(feedIds, request.UserId);

            var viewModels = new List<FeedViewModel>();
            foreach (var feed in feeds)
            {
                viewModels.Add(new FeedViewModel
                {
                    Id = feed.Id,
                    UserId = feed.UserId,
                    Content = feed.Content,
                    Medias = feed.Medias,
                    CreatedAt = feed.CreatedAt,
                    ModifiedAt = feed.ModifiedAt,
                    LikeCount = likeCounts.ContainsKey(feed.Id) ? likeCounts[feed.Id] : 0,
                    IsLiked = likeStatuses.ContainsKey(feed.Id) && likeStatuses[feed.Id]
                });
            }
            return LiftNetRes<FeedViewModel>.SuccessResponse(viewModels);
        }
    }
} 