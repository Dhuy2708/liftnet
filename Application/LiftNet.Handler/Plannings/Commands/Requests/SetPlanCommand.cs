using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Plannings.Commands.Requests
{
    public class SetPlanCommand : IRequest<LiftNetRes>
    {
        public string UserId
        {
            get; set;
        }
        public Dictionary<int, List<string>> DayWithExerciseIds
        {
            get; set;
        }
    }
}
