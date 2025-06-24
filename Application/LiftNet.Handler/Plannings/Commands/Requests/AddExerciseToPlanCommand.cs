using LiftNet.Domain.Response;
using MediatR;

namespace LiftNet.Handler.Plannings.Commands.Requests
{
    public class AddExerciseToPlanCommand : IRequest<LiftNetRes>
    {
        public string UserId { get; set; }
        public int DayOfWeek { get; set; }
        public string ExerciseId { get; set; }
    }
} 