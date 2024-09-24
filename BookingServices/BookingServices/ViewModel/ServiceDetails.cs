namespace BookingServices.ViewModel
{
    public class ServiceDetails
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string Details { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? Quantity { get; set; }
        public List<string> ImageUrl { get; set; }
    }
}
