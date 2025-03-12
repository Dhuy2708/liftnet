namespace LiftNet.Contract.Views
{
    public class AddressView
    {
        public ProvinceView Province
        {
            get; set;
        }
        public DistrictView District
        {
            get; set;
        }
        public WardView Ward
        {
            get; set;
        }
        public string Address
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
