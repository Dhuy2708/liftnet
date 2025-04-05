using System.Collections.Generic;

namespace LiftNet.MapSDK.Contracts.Common
{
    public class AddressComponent
    {
        public string LongName { get; set; }
        public string ShortName { get; set; }
    }

    public class Geometry
    {
        public Location Location { get; set; }
    }

    public class Location
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class PlusCode
    {
        public string CompoundCode { get; set; }
        public string GlobalCode { get; set; }
    }
}
