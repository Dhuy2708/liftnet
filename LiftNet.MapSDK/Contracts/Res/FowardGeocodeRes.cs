using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.MapSDK.Contracts.Res
{
    public class ForwardGeocodeRes
    {
        public PlusCodeInfo PlusCode { get; set; }
        public List<ForwardGeocodeResult> Results { get; set; }
        public string Status { get; set; }
    }

    public class PlusCodeInfo
    {
        public string CompoundCode { get; set; }
        public string GlobalCode { get; set; }
    }

    public class ForwardGeocodeResult
    {
        public List<AddressComponent> AddressComponents { get; set; }
        public string FormattedAddress { get; set; }
        public GeometryInfo Geometry { get; set; }
        public string PlaceId { get; set; }
        public string Reference { get; set; }
        public PlusCodeInfo PlusCode { get; set; }
        public List<string> Types { get; set; }
    }

    public class AddressComponent
    {
        public string LongName { get; set; }
        public string ShortName { get; set; }
    }

    public class GeometryInfo
    {
        public LatLng Location { get; set; }
    }

    public class LatLng
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
