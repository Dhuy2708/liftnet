using LiftNet.Domain.Response;
using MediatR;

namespace LiftNet.Handler.Plannings.Commands.Requests
{
    public class SetPhysicalStatsCommand : IRequest<LiftNetRes>
    {
        public string UserId { get; set; }
        public int? Age { get; set; }
        public int? Gender { get; set; }
        public int? Height { get; set; }
        public float? Mass { get; set; }
        public float? Bdf { get; set; }
        public int? ActivityLevel { get; set; }
        public int? Goal { get; set; }
    }
} 