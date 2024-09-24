using BookingServices.Data;

namespace BookingServices.Models
{
    public class bookingViewModel
    {
        public List<Booking> Bookings { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalCanceled { get; set; }
        public decimal TotalPending { get; set; }


        public int BookingId { get; set; }
        public DateTime EventDate { get; set; }
        public string Status { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string CustomerName { get; set; }
        public string PaymentIncomeDescription { get; set; }
        //this is model view 
    }
}
