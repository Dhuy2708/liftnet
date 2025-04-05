using Newtonsoft.Json;

namespace LiftNet.Domain.Indexes
{
    public class AddressIndexData : IndexData
    {
        [JsonProperty(PropertyName = "placename")]
        public string PlaceName { get; set; }

        [JsonProperty(PropertyName = "formattedaddress")]
        public string FormattedAddress { get; set; }

        [JsonProperty(PropertyName = "lat")]
        public double Latitude { get; set; }

        [JsonProperty(PropertyName = "lng")]
        public double Longitude { get; set; }

        [JsonProperty(PropertyName = "placeid")]
        public string PlaceId { get; set; }
    }
}
