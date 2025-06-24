using LiftNet.Domain.Response;
using MediatR;

namespace LiftNet.Handler.Plannings.Commands.Requests
{
    public class ReorderExerciseCommand : IRequest<LiftNetRes>
    {
        public string UserId { get; set; }
        public int SourceDayOfWeek { get; set; }
        public int SourceOrder { get; set; }
        public int TargetDayOfWeek { get; set; }
        public int TargetOrder { get; set; }
        public string ExerciseId { get; set; }
    }
} 