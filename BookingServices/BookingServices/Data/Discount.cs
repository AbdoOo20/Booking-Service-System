using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Data
{
    public class Discount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiscountId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(0.0, 20.0)]
        public decimal Percentage { get; set; }

        [ForeignKey("PaymentIncome")]
        public int PaymentIncomeId { get; set; }

        public virtual PaymentIncome? PaymentIncome { get; set; }
    }
}
