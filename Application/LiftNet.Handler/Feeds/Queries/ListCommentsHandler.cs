using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Contract.Views.Feeds;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Feeds.Queries.Requests;
using LiftNet.Utility.Utils;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Feeds.Queries
{
    public class ListCommentsHandler : IRequestHandler<ListCommentsQuery, LiftNetRes<CommentView>>
    {
        private readonly ILiftLogger<ListCommentsHandler> _logger;
        private readonly IFeedIndexService _feedService;
        private readonly IUserService _userService;

        public ListCommentsHandler(ILiftLogger<ListCommentsHandler> logger, 
                                   IFeedIndexService feedService, 
                                   IUserService userService)
        {
            _logger = logger;
            _feedService = feedService;
            _userService = userService;
        }

        public async Task<LiftNetRes<CommentView>> Handle(ListCommentsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var comments = await _feedService.ListCommentsAsync(request.FeedId, request.ParentId);
                var userOverviews = await _userService.GetOverviewsDictByIds(comments.Select(c => c.UserId).Distinct().ToList());
                var commentViews = comments.Select(c => new CommentView
                {
                    Id = c.Id,
                    User = userOverviews.ContainsKey(c.UserId) ? userOverviews[c.UserId] : null,
                    Comment = c.Comment,
                    CreatedAt = c.CreatedAt.ToOffSet(),
                    ModifiedAt = c.ModifiedAt.ToOffSet()
                }).ToList();
                return LiftNetRes<CommentView>.SuccessResponse(commentViews);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error while listing comments for feed {request.FeedId} with parent {request.ParentId}");
                return LiftNetRes<CommentView>.ErrorResponse("An error occurred while listing comments.");
            }
        }
    }
}
