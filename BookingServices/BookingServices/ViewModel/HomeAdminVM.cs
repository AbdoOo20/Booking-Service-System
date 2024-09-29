namespace BookingServices.ViewModel
{
    public class HomeAdminVM
    {
        public int CustomerCount { get; set; }
        public int ProvidersCount { get; set; }
        public int ServicesCount { get; set; }
        public int ServicesOffLineCount { get; set; }
        public int ServicesOnLineCount { get; set; }
        public decimal TotalMoneyService { get; set; }
    }
}
