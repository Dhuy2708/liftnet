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
        private readonly IFeedIndexService _feedService;

        public CommentFeedHandler(ILiftLogger<CommentFeedHandler> logger, IFeedIndexService feedService)
        {
            _logger = logger;
            _feedService = feedService;
        }

        public async Task<LiftNetRes> Handle(CommentFeedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _feedService.CommentFeedAsync(
                                request.FeedId,
                                request.CallerId,
                                request.Comment,
                                request.ParentId);
                if (result)
                {
                    return LiftNetRes.SuccessResponse("Comment added successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while handling comment feed command");
            }
            return LiftNetRes.ErrorResponse("Failed to comment");
        }
    }
}
