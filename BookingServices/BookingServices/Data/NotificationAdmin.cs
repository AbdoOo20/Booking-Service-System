namespace BookingServices.Data
{
    public class NotificationAdmin
    {
        public int Id { get; set; }
        public string? NotificationTitle { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
