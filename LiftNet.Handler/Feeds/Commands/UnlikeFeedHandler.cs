using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using LiftNet.Domain.Response;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Domain.Interfaces;
using LiftNet.Handler.Feeds.Commands.Requests;

namespace LiftNet.Handler.Feeds.Commands
{
    public class UnlikeFeedHandler : IRequestHandler<UnlikeFeedCommand, LiftNetRes>
    {
        private readonly IFeedIndexService _feedService;
        private readonly ILiftLogger<UnlikeFeedHandler> _logger;

        public UnlikeFeedHandler(IFeedIndexService feedService, ILiftLogger<UnlikeFeedHandler> logger)
        {
            _feedService = feedService;
            _logger = logger;
        }

        public async Task<LiftNetRes> Handle(UnlikeFeedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _feedService.UnlikeFeedAsync(request.FeedId, request.UserId);
                if (!result)
                    return LiftNetRes.ErrorResponse("Failed to unlike feed or not liked");

                return LiftNetRes.SuccessResponse("Feed unliked successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in UnlikeFeedHandler");
                return LiftNetRes.ErrorResponse("Internal server error");
            }
        }
    }
} 