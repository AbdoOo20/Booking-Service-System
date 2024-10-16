using System.ComponentModel.DataAnnotations;

namespace BookingServices.Data
{
    public class RemainingCustomerBalance
    {
        [Key]
        public string BankAccount { get; set; }
        public decimal RemainingAmount { get; set; }
    }
}
