using LiftNet.Domain.Response;
using MediatR;

public class UnlikeFeedCommand : IRequest<LiftNetRes>
{
    public string FeedId { get; set; }
    public string UserId { get; set; }
} 