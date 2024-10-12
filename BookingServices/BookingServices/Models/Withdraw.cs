namespace BookingServices.Models
{
    public class Withdraw
    {
        public string ServiceProviderEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PlatformPercentage { get; set; }
    }
}
