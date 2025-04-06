using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Handler.Feeds.Commands.Requests;
using LiftNet.Domain.Response;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Interfaces;

namespace LiftNet.Handler.Feeds.Commands
{
    public class UpdateFeedHandler : IRequestHandler<UpdateFeedCommand, LiftNetRes<FeedIndexData>>
    {
        private readonly IFeedIndexService _feedService;
        private readonly ILiftLogger<UpdateFeedHandler> _logger;

        public UpdateFeedHandler(IFeedIndexService feedService, ILiftLogger<UpdateFeedHandler> logger)
        {
            _feedService = feedService;
            _logger = logger;
        }

        public async Task<LiftNetRes<FeedIndexData>> Handle(UpdateFeedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var feed = await _feedService.UpdateFeedAsync(request.Id, request.UserId, request.Content, request.Medias);
                if (feed == null)
                    return LiftNetRes<FeedIndexData>.ErrorResponse("Feed not found or unauthorized");

                return LiftNetRes<FeedIndexData>.SuccessResponse(feed);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in UpdateFeedHandler");
                return LiftNetRes<FeedIndexData>.ErrorResponse("Internal server error");
            }
        }
    }
} 