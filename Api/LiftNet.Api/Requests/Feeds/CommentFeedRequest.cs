namespace LiftNet.Api.Requests.Feeds
{
    public class CommentFeedRequest
    {
        public string FeedId
        {
            get; set;
        }
        public string Comment
        {
            get; set;
        }
        public string? ParentId
        {
            get; set;
        }
    }
}
