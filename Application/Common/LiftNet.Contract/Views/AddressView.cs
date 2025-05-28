namespace LiftNet.Contract.Views
{
    public class AddressView
    {
        public string PlaceName
        {
            get; set;
        }
        public string ShortPlaceName
        {
            get; set;
        }
        public double Lat
        {
            get; set;
        }
        public double Lng
        {
            get; set;
        }
        public string PlaceId
        {
            get; set;
        }
    }

    public class ProvinceView
    {
        public int Code
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public int PhoneCode
        {
            get; set;
        }
    }

    public class DistrictView
    {
        public int Code
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
    }

    public class WardView
    {
        public int Code
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
    }
}
