using LiftNet.Domain.Response;
using MediatR;

namespace LiftNet.Handler.Plannings.Commands.Requests
{
    public class UpdateSpecificDaysPlanCommand : IRequest<LiftNetRes>
    {
        public string UserId { get; set; }
        public List<int> DaysOfWeek { get; set; }
        public Dictionary<int, List<string>> DayWithExerciseIds { get; set; }
    }
} 