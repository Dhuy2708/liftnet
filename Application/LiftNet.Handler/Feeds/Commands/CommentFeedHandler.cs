using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Feeds.Commands.Requests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Feeds.Commands
{
    public class CommentFeedHandler : IRequestHandler<CommentFeedCommand, LiftNetRes>
    {
        private readonly ILiftLogger<CommentFeedHandler> _logger;
        private readonly IIndexBaseService<CommentIndexData> _commentService;
        private readonly IIndexBaseService<FeedIndexData> _feedService;

        public CommentFeedHandler(ILiftLogger<CommentFeedHandler> logger, 
                                  IIndexBaseService<CommentIndexData> commentService, 
                                  IIndexBaseService<FeedIndexData> feedService)
        {
            _logger = logger;
            _commentService = commentService;
            _feedService = feedService;
        }

        public async Task<LiftNetRes> Handle(CommentFeedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var now = DateTime.UtcNow;

                var condition = new QueryCondition();
                condition.AddCondition(new ConditionItem("id", request.FeedId));
                condition.AddCondition(new ConditionItem(DataSchema.Comment));
                var feed = await _feedService.QueryAsync(condition);
                if (feed.Items.Count == 0)
                {
                    _logger.Warn($"Feed with ID {request.FeedId} not found for user {request.CallerId}.");
                    return LiftNetRes.ErrorResponse("Feed not found.");
                }

                var index = new CommentIndexData
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = request.CallerId,
                    Schema = DataSchema.Comment,
                    CreatedAt = now,
                    ModifiedAt = now,
                    FeedId = request.FeedId,
                    Comment = request.Comment,  
                    ParentId = request.ParentId,
                };

                await _commentService.UpsertAsync(index);
                return LiftNetRes.SuccessResponse("Comment added successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while handling comment feed command");
                return LiftNetRes.ErrorResponse("An error occurred while processing your request.");
            }
        }
    }
}
