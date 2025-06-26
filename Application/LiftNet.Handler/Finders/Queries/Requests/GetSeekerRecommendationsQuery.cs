using LiftNet.Contract.Views.Finders;
using LiftNet.Domain.Response;
using MediatR;

namespace LiftNet.Handler.Finders.Queries.Requests
{
    public class GetSeekerRecommendationsQuery : IRequest<LiftNetRes<List<SeekerRecommendationView>>>
    {
        public string CoachId { get; set; }
    }
} 