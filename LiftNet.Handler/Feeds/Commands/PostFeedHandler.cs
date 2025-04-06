using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Feeds.Commands.Requests;
using LiftNet.Domain.Indexes;

namespace LiftNet.Handler.Feeds.Commands
{
    public class PostFeedHandler : IRequestHandler<PostFeedCommand, LiftNetRes<FeedIndexData>>
    {
        private readonly IFeedIndexService _feedService;
        private readonly ILiftLogger<PostFeedHandler> _logger;

        public PostFeedHandler(IFeedIndexService feedService, ILiftLogger<PostFeedHandler> logger)
        {
            _feedService = feedService;
            _logger = logger;
        }

        public async Task<LiftNetRes<FeedIndexData>> Handle(PostFeedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var feed = await _feedService.PostFeedAsync(request.UserId, request.Content, request.Medias);
                if (feed == null)
                    return LiftNetRes<FeedIndexData>.ErrorResponse("Failed to post feed");

                return LiftNetRes<FeedIndexData>.SuccessResponse(feed);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in PostFeedHandler");
                return LiftNetRes<FeedIndexData>.ErrorResponse("Internal server error");
            }
        }
    }
} 