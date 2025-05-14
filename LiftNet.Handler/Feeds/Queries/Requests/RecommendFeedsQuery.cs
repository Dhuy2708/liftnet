using MediatR;
using LiftNet.Domain.Response;
using LiftNet.Domain.ViewModels;
using System.Collections.Generic;

namespace LiftNet.Handler.Feeds.Queries.Requests
{
    public class RecommendFeedsQuery : IRequest<LiftNetRes<FeedViewModel>>
    {
        public string UserId { get; set; }
    }
} 