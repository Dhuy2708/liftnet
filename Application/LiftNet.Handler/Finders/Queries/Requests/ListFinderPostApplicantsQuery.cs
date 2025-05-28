using LiftNet.Contract.Views.Finders;
using LiftNet.Domain.Response;
using MediatR;
using System.Collections.Generic;

namespace LiftNet.Handler.Finders.Queries.Requests
{
    public class ListFinderPostApplicantsQuery : IRequest<LiftNetRes<FinderPostApplicantView>>
    {
        public string UserId { get; set; }
        public string PostId { get; set; }
    }
} 