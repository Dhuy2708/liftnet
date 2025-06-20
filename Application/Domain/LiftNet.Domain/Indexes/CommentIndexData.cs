using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Indexes
{
    public class CommentIndexData : IndexData
    {
        [JsonProperty(PropertyName = "feedid")]
        public string FeedId
        {
            get; set;
        }

        [JsonProperty(PropertyName = "comment")]
        public string Comment
        {
            get; set;
        }

        [JsonProperty(PropertyName = "isroot")]
        public bool IsRoot
        {
            get; set;
        }

        [JsonProperty(PropertyName = "parentid", NullValueHandling = NullValueHandling.Ignore)]
        public string? ParentId
        {
            get; set;
        }
    }
}
