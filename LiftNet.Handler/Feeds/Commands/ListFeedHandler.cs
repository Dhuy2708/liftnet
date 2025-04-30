using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using MediatR;
using Microsoft.Extensions.Logging;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Feeds.Commands.Requests;
using LiftNet.Domain.Indexes;
using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Enums;
using LiftNet.Domain.ViewModels;

namespace LiftNet.Handler.Feeds.Commands
{
    public class ListFeedHandler : IRequestHandler<ListFeedCommand, LiftNetRes<FeedViewModel>>
    {
        private readonly IFeedIndexService _feedService;
        private readonly ILiftLogger<ListFeedHandler> _logger;

        public ListFeedHandler(
            IFeedIndexService feedService,
            ILiftLogger<ListFeedHandler> logger)
        {
            _feedService = feedService;
            _logger = logger;
        }

        public async Task<LiftNetRes<FeedViewModel>> Handle(ListFeedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var condition = request.QueryCondition;
                condition.AddCondition(new ConditionItem("userid", new List<string> { request.UserId }, FilterType.String));
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
                
                var viewModels = new List<FeedViewModel>();
                foreach (var feed in feeds)
                {
                    var likeCount = await _feedService.GetFeedLikeCountAsync(feed.Id);
                    var isLiked = await _feedService.HasUserLikedFeedAsync(feed.Id, request.UserId);
                    
                    viewModels.Add(new FeedViewModel
                    {
                        Id = feed.Id,
                        UserId = feed.UserId,
                        Content = feed.Content,
                        Medias = feed.Medias,
                        CreatedAt = feed.CreatedAt,
                        ModifiedAt = feed.ModifiedAt,
                        LikeCount = likeCount,
                        IsLiked = isLiked
                    });
                }

                return LiftNetRes<FeedViewModel>.SuccessResponse(viewModels, nextPageToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in ListFeedHandler");
                return LiftNetRes<FeedViewModel>.ErrorResponse("Internal server error");
            }
        }
    }
} 