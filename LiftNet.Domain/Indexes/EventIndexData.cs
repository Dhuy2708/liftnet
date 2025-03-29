using LiftNet.Domain.Enums;
using LiftNet.Domain.Enums.Indexes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Indexes
{
    public class EventIndexData : IndexData
    {
        [JsonProperty(PropertyName = "title")]
        public string Title
        {
            get; set;
        }

        [JsonProperty(PropertyName = "description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description
        {
            get; set;
        }

        [JsonProperty(PropertyName = "color", NullValueHandling = NullValueHandling.Ignore)]
        public string Color
        {
            get; set;
        } = LiftNetColor.DEFAULT;

        [JsonProperty(PropertyName = "starttime")]
        public DateTime StartTime
        {
            get; set;
        }

        [JsonProperty(PropertyName = "endtime")]
        public DateTime EndTime
        {
            get; set;
        }

        [JsonProperty(PropertyName = "appointmentid", NullValueHandling = NullValueHandling.Ignore)]
        public string AppointmentId
        {
            get; set;
        }

        [JsonProperty(PropertyName = "rule", NullValueHandling = NullValueHandling.Ignore)]
        public RepeatRule Rule
        {
            get; set;
        }
    }

    public class ParticipantIndexData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get; set;
        }

        [JsonProperty(PropertyName = "username")]
        public string Username
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

        [JsonProperty(PropertyName = "role")]
        public LiftNetRoleEnum Role
        {
            get; set;
        }

        [JsonProperty(PropertyName = "avatar")]
        public string Avatar
        {
            get; set;
        }

        [JsonProperty(PropertyName = "status")]
        public ParticipantStatus Status
        {
            get; set;
        }
    }
}
