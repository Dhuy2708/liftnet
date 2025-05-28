using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Indexes
{
    public class UserIndexData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get; set;
        }

        [JsonProperty(PropertyName = "username")]
        public string UserId
        {
            get; set;
        }

        [JsonProperty(PropertyName = "email")]
        public string Email
        {
            get; set;
        }

        [JsonProperty(PropertyName = "firstname")]
        public string FirstName
        {
            get; set;
        }

        [JsonProperty(PropertyName = "lastname")]
        public string LastName
        {
            get; set;
        }

        [JsonProperty(PropertyName = "avatar")]
        public string Avatar
        {
            get; set;
        }
    }
}
