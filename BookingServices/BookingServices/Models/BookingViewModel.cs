using BookingServices.Data;

namespace BookingServices.Models
{
    public class BookingViewModel
    {
        public int BookingId { get; set; }
        public DateTime EventDate { get; set; }
        public string Status { get; set; }
        public string? Type { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string CustomerName { get; set; }
        public string ServiceName { get; set; }
        public string PaymentIncome { get; set; }
        public DateTime BookDate { get; set; }
    }
}
