namespace BookingServices.Models
{
    public class PaymentReportViewModel
    {
        public string Name { get; set; }
        public int PaymentCount { get; set; }
        public decimal TotalBenefit { get; set; }
        public DateTime? BookingDate { get; set; }
        public int PaymentIncomeId { get; set; }
    }
}
