using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Indexes
{
    public class FeedIndexData : IndexData
    {
        [JsonProperty(PropertyName = "content")]
        public string Content
        {
            get; set;
        }

        [JsonProperty(PropertyName = "medias")]
        public List<string> Medias
        {
            get; set;
        }

        [JsonProperty(PropertyName = "rand")]
        public float Rand
        {
            get; set;
        } = (float)new Random().NextDouble();
    }
}
