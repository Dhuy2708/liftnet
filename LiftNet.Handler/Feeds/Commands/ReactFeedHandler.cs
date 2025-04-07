using LiftNet.Contract.Constants;
using LiftNet.Contract.Enums;
using LiftNet.Contract.Enums.Feed;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Feeds.Commands.Requests;
using LiftNet.ServiceBus.Contracts;
using LiftNet.ServiceBus.Interfaces;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiftNet.Handler.Feeds.Commands
{
    public class ReactFeedHandler : IRequestHandler<ReactFeedCommand, LiftNetRes>
    {
        private readonly IEventBusService _eventBusService;
        private readonly ILiftLogger<ReactFeedHandler> _logger;

        public ReactFeedHandler(IEventBusService eventBusService, ILiftLogger<ReactFeedHandler> logger)
        {
            _eventBusService = eventBusService;
            _logger = logger;
        }

        public async Task<LiftNetRes> Handle(ReactFeedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var context = new
                {
                    FeedId = request.FeedId,
                    UserId = request.UserId,
                };

                var eventMessage = new EventMessage
                {
                    Type = request.Type == ReactType.Like ? EventType.LikeFeed : EventType.UnLikeFeed,
                    Context = JsonConvert.SerializeObject(context)
                };

                await _eventBusService.PublishAsync(eventMessage, QueueNames.Feed);
                return LiftNetRes.SuccessResponse($"Feed {request.Type.ToString().ToLower()} successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in ReactFeedHandler");
                return LiftNetRes.ErrorResponse("Internal server error");
            }
        }
    }
}