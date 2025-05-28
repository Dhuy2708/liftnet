namespace LiftNet.Contract.Dtos
{
    public class PlaceDetailDto
    {
        public string PlaceName { get; set; }
        public string PlaceId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string FormattedAddress { get; set; }
    }
}
