using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Socials.Queries.Requests;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace LiftNet.Handler.Socials.Queries
{
    public class SearchPrioritizedUserHandler : IRequestHandler<SearchPrioritizedUserQuery, PaginatedLiftNetRes<UserOverview>>
    {
        private readonly IRecommendationService _recommendationService;
        private readonly ILiftLogger<SearchPrioritizedUserHandler> _logger;

        public SearchPrioritizedUserHandler(
            IRecommendationService recommendationService,
            ILiftLogger<SearchPrioritizedUserHandler> logger)
        {
            _recommendationService = recommendationService;
            _logger = logger;
        }

        public async Task<PaginatedLiftNetRes<UserOverview>> Handle(SearchPrioritizedUserQuery request, CancellationToken cancellationToken)
        {
            _logger.Info("Begin searching prioritized users");
            try
            {
                var conditions = request.Conditions ?? new QueryCondition();
                var users = await _recommendationService.SearchPrioritizedUser(
                    request.UserId,
                    conditions.Search ?? string.Empty,
                    10,
                    conditions.PageNumber
                );

                _logger.Info("Search prioritized users successfully");
                return PaginatedLiftNetRes<UserOverview>.SuccessResponse(
                    users,
                    conditions.PageNumber,
                    10,
                    users.Count
                );
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex, "Error searching prioritized users");
                return PaginatedLiftNetRes<UserOverview>.ErrorResponse("Error searching users");
            }
        }
    }
} 