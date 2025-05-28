using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiftNet.MapSDK.Contracts.Common;

namespace LiftNet.MapSDK.Contracts.Res
{
    public class ForwardGeocodeRes
    {
        public PlusCode PlusCode { get; set; }
        public List<ForwardGeocodeResult> Results { get; set; }
        public string Status { get; set; }
    }

    public class ForwardGeocodeResult
    {
        public List<AddressComponent> AddressComponents { get; set; }
        public string FormattedAddress { get; set; }
        public Geometry Geometry { get; set; }
        public string PlaceId { get; set; }
        public string Reference { get; set; }
        public PlusCode PlusCode { get; set; }
        public List<string> Types { get; set; }
    }
}
