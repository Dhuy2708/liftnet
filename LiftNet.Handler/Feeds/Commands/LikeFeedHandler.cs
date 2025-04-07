using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using LiftNet.Domain.Indexes;
using LiftNet.Handler.Feeds.Commands.Requests;
using LiftNet.Domain.Response;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Domain.Interfaces;

namespace LiftNet.Handler.Feeds.Commands
{
    public class LikeFeedHandler : IRequestHandler<LikeFeedCommand, LiftNetRes>
    {
        private readonly IFeedIndexService _feedService;
        private readonly ILiftLogger<LikeFeedHandler> _logger;

        public LikeFeedHandler(IFeedIndexService feedService, ILiftLogger<LikeFeedHandler> logger)
        {
            _feedService = feedService;
            _logger = logger;
        }

        public async Task<LiftNetRes> Handle(LikeFeedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _feedService.LikeFeedAsync(request.FeedId, request.UserId);
                if (!result)
                    return LiftNetRes.ErrorResponse("Failed to like feed or already liked");

                return LiftNetRes.SuccessResponse("Feed liked successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in LikeFeedHandler");
                return LiftNetRes.ErrorResponse("Internal server error");
            }
        }
    }
} 