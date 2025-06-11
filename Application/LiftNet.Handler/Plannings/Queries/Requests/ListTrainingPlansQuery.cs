using LiftNet.Contract.Views.Plannings;
using LiftNet.Domain.Response;
using MediatR;

namespace LiftNet.Handler.Plannings.Queries.Requests
{
    public class ListTrainingPlansQuery : IRequest<PaginatedLiftNetRes<TrainingPlanView>>
    {
        public string UserId { get; set; }
    }
} 