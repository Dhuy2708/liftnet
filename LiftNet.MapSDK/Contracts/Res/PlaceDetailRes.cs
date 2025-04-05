using LiftNet.MapSDK.Contracts.Common;

namespace LiftNet.MapSDK.Contracts.Res
{
    public class PlaceDetailRes
    {
        public PlaceDetailResult Result { get; set; }
        public string Status { get; set; }
    }

    public class PlaceDetailResult
    {
        public string PlaceId { get; set; }
        public string FormattedAddress { get; set; }
        public Geometry Geometry { get; set; }
        public string Name { get; set; }
    }
}
