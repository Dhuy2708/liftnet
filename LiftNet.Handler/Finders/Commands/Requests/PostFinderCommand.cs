using LiftNet.Contract.Enums.Appointment;
using LiftNet.Contract.Enums;
using LiftNet.Domain.Response;
using LiftNet.Handler.Auths.Commands.Requests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Finders.Commands.Requests
{
    public class PostFinderCommand : IRequest<LiftNetRes>
    {
        public string UserId
        {
            get; set;
        }

        public string Title
        {
            get; set;
        }

        public bool IsAnonymous
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public DateTime StartTime
        {
            get; set;
        }

        public DateTime EndTime
        {
            get; set;
        }

        public int StartPrice
        {
            get; set;
        }

        public int EndPrice
        {
            get; set;
        }

        public string LocationId
        {
            get; set;
        }

        public bool HideAddress
        {
            get; set;
        }

        public RepeatingType RepeatType
        {
            get; set;
        }
    }
}
