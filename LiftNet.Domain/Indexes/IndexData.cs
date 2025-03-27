using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Indexes
{
    public class IndexData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get; set;
        } = Guid.NewGuid().ToString();

        [JsonProperty(PropertyName = "userid")]
        public string UserId
        {
            get; set;
        }

        [JsonProperty(PropertyName = "schema", NullValueHandling = NullValueHandling.Ignore)]
        public DataSchema Schema
        {
            get; set;
        }

        [JsonProperty(PropertyName = "created")]
        public DateTime CreatedAt
        {
            get; set;
        }

        [JsonProperty(PropertyName = "modified")]
        public DateTime ModifiedAt
        {
            get; set;
        }
    }

    public enum DataSchema
    {
        None = 0,

        // feed
        Feed = 1,
        Comment = 2,

        // chat
        Chat = 10,

    }
}
