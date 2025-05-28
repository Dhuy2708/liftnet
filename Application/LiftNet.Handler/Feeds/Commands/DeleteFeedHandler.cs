using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Handler.Feeds.Commands.Requests;
using LiftNet.Domain.Response;
using LiftNet.Domain.Interfaces;

namespace LiftNet.Handler.Feeds.Commands
{
    public class DeleteFeedHandler : IRequestHandler<DeleteFeedCommand, LiftNetRes>
    {
        private readonly IFeedIndexService _feedService;
        private readonly ILiftLogger<DeleteFeedHandler> _logger;

        public DeleteFeedHandler(IFeedIndexService feedService, ILiftLogger<DeleteFeedHandler> logger)
        {
            _feedService = feedService;
            _logger = logger;
        }

        public async Task<LiftNetRes> Handle(DeleteFeedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _feedService.DeleteFeedAsync(request.Id, request.UserId);
                if (!result)
                    return LiftNetRes.ErrorResponse("Feed not found or unauthorized");

                return LiftNetRes.SuccessResponse("Feed deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in DeleteFeedHandler");
                return LiftNetRes.ErrorResponse("Internal server error");
            }
        }
    }
} 