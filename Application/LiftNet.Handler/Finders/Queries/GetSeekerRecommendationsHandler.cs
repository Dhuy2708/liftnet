using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Views.Finders;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Finders.Queries.Requests;
using LiftNet.Utility.Extensions;
using LiftNet.Utility.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftNet.Handler.Finders.Queries
{
    public class GetSeekerRecommendationsHandler : IRequestHandler<GetSeekerRecommendationsQuery, LiftNetRes<List<SeekerRecommendationView>>>
    {
        private readonly ILiftLogger<GetSeekerRecommendationsHandler> _logger;
        private readonly IUnitOfWork _uow;

        public GetSeekerRecommendationsHandler(ILiftLogger<GetSeekerRecommendationsHandler> logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        public async Task<LiftNetRes<List<SeekerRecommendationView>>> Handle(GetSeekerRecommendationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.CoachId.IsNullOrEmpty())
                {
                    return LiftNetRes<List<SeekerRecommendationView>>.ErrorResponse("CoachId cannot be null or empty.");
                }

                var recommendations = await _uow.CoachRecommendationRepo.GetQueryable()
                    .Include(r => r.Seeker)
                    .Where(r => r.CoachId == request.CoachId)
                    .OrderByDescending(r => r.LastUpdated)
                    .ToListAsync(cancellationToken);

                var seekerRecommendationViews = recommendations.Select(r => new SeekerRecommendationView
                {
                    Seeker = new UserOverview
                    {
                        Id = r.Seeker.Id,
                        FirstName = r.Seeker.FirstName,
                        LastName = r.Seeker.LastName,
                        Avatar = r.Seeker.Avatar,
                        Email = r.Seeker.Email,
                    },
                    RecommendedAt = r.LastUpdated.ToOffSet(),
                    Description = r.Description
                }).ToList();

                return LiftNetRes<List<SeekerRecommendationView>>.SuccessResponse(seekerRecommendationViews);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while getting seeker recommendations");
                return LiftNetRes<List<SeekerRecommendationView>>.ErrorResponse("An error occurred while processing your request.");
            }
        }
    }
} 