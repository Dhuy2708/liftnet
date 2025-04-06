using System.Collections.Generic;
using MediatR;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Response;
using Microsoft.AspNetCore.Http;

namespace LiftNet.Handler.Feeds.Commands.Requests
{
    public class PostFeedCommand : IRequest<LiftNetRes<FeedIndexData>>
    {
        public string UserId { get; set; }
        public string Content { get; set; }
        public List<IFormFile> MediaFiles { get; set; }
    }
} 