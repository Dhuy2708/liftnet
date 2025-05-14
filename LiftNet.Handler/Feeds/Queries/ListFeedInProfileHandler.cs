using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Enums;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Domain.ViewModels;
using LiftNet.Handler.Feeds.Queries.Requests;
using MediatR;

namespace LiftNet.Handler.Feeds.Queries
{
    public class ListFeedInProfileHandler : IRequestHandler<ListFeedInProfileQuery, PaginatedLiftNetRes<FeedViewModel>>
    {
        private readonly IFeedIndexService _feedService;
        private readonly ILiftLogger<ListFeedInProfileHandler> _logger;

        public ListFeedInProfileHandler(
            IFeedIndexService feedService,
            ILiftLogger<ListFeedInProfileHandler> logger)
        {
            _feedService = feedService;
            _logger = logger;
        }

        public async Task<PaginatedLiftNetRes<FeedViewModel>> Handle(ListFeedInProfileQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.Info($"list feeds in profile, userId: {request.ProfileId}");
                var condition = request.QueryCondition;
                condition.AddCondition(new ConditionItem("userid", new List<string> { request.ProfileId }, FilterType.String));
                condition.AddCondition(new ConditionItem("schema", new List<string> { $"{(int)DataSchema.Feed}" }, FilterType.Integer, QueryOperator.Equal, QueryLogic.And));

                if (condition.Sort == null)
                {
                    condition.Sort = new SortCondition
                    {
                        Name = "created",
                        Type = SortType.Desc
                    };
                }

                var (feeds, nextPageToken) = await _feedService.QueryAsync(condition);

                var feedIds = feeds.Select(f => f.Id).ToList();
                var likeCounts = await _feedService.GetFeedLikeCountsAsync(feedIds);
                var likeStatuses = await _feedService.GetFeedLikeStatusesAsync(feedIds, request.UserId);

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
                        LikeCount = likeCounts[feed.Id],
                        IsLiked = likeStatuses[feed.Id]
                    });
                }

                return PaginatedLiftNetRes<FeedViewModel>.SuccessResponse(viewModels, nextPageToken: nextPageToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in ListFeedInProfileHandler");
                return PaginatedLiftNetRes<FeedViewModel>.ErrorResponse("Internal server error");
            }
        }
    }
}