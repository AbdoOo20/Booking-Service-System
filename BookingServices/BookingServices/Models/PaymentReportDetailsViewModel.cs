namespace BookingServices.Models
{
    public class PaymentReportDetailsViewModel
    {
        public string CustomerName { get; set; }
        public string ProviderName { get; set; }
        public string ServiceName { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime BookingData {  get; set; }
        public decimal BookingPrice { get; set; }
    }
}
