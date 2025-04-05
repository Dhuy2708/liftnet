using System.Collections.Generic;
using LiftNet.MapSDK.Contracts.Common;

namespace LiftNet.MapSDK.Contracts.Res
{
    public class ReverseGeocodeRes
    {
        public List<ReverseGeocodeResult>? Results { get; set; }
        public string Status { get; set; }
    }

    public class ReverseGeocodeResult
    {
        public List<AddressComponent> AddressComponents { get; set; }
        public string FormattedAddress { get; set; }
        public Geometry Geometry { get; set; }
        public string PlaceId { get; set; }
        public PlusCode PlusCode { get; set; }
    }
}
