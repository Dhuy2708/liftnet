using System.ComponentModel.DataAnnotations;

namespace LiftNet.Api.Requests.Plannings
{
    public class SetPhysicalStatsReq
    {
        public int? Age { get; set; }
        public int? Gender { get; set; }
        public int? Height { get; set; }
        public float? Mass { get; set; }
        public float? Bdf { get; set; }
        public int? ActivityLevel { get; set; }
        public int? Goal { get; set; }
    }
} 