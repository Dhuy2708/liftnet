using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Exceptions;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Socials.Queries.Requests;
using LiftNet.Utility.Extensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Socials.Queries
{
    public class ListFriendSuggestionHandler : IRequestHandler<FriendSuggestionQuery, LiftNetRes<UserOverview>>
    {
        private readonly ILiftLogger<ListFriendSuggestionHandler> _logger;
        private readonly IRecommendationService _recommendationService;

        public ListFriendSuggestionHandler(ILiftLogger<ListFriendSuggestionHandler> logger, IRecommendationService recommendationService)
        {
            _logger = logger;
            _recommendationService = recommendationService;
        }

        public async Task<LiftNetRes<UserOverview>> Handle(FriendSuggestionQuery request, CancellationToken cancellationToken)
        {
            _logger.Info("begin suggest friends");
            try
            {
                var userId = request.UserId;
                if (userId.IsNullOrEmpty())
                {
                    throw new BadRequestException(["user id is null or empty"]);
                }
                var result = await _recommendationService.SuggestFriendsAsync(userId);
                return LiftNetRes<UserOverview>.SuccessResponse(result);
            }
            catch (Exception e)
            {
                _logger.Error(e, "error while suggesting friends");
                return LiftNetRes<UserOverview>.ErrorResponse("error while suggesting friends");
            }
        }
    }
}
