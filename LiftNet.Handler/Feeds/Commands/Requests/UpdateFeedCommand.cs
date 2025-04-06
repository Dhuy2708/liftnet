using System.Collections.Generic;
using MediatR;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Response;

namespace LiftNet.Handler.Feeds.Commands.Requests
{
    public class UpdateFeedCommand : IRequest<LiftNetRes<FeedIndexData>>
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public List<string> Medias { get; set; }
    }
} 