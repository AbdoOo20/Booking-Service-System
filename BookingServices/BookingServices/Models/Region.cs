namespace BookingServices.Models
{
    public class Region
    {
        public int region_id { get; set; }
        public int capital_city_id { get; set; }
        public string? code { get; set; }
        public string? name_ar { get; set; }
        public string? name_en { get; set; }
        public double population { get; set; }
    }
}
