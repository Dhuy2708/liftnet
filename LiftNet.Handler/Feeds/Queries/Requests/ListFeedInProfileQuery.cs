using MediatR;
using LiftNet.Domain.Response;
using LiftNet.Contract.Dtos.Query;
using LiftNet.Domain.ViewModels;

namespace LiftNet.Handler.Feeds.Queries.Requests
{
    public class ListFeedInProfileQuery : IRequest<PaginatedLiftNetRes<FeedViewModel>>
    {

        public string UserId
        {
            get; set;
        }
        public string ProfileId 
        {
            get; set; 
        }
        public QueryCondition QueryCondition 
        { 
            get; set; 
        }
    }
} 