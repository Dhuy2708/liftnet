using Newtonsoft.Json;

namespace LiftNet.Domain.Indexes
{
    public class LikeIndexData : IndexData
    {
        [JsonProperty(PropertyName = "feedid")]
        public string FeedId { get; set; }
    }
} 