using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingServices.Data
{
    public class PaymentIncome
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentIncomeId { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        [Required]
        [Range(0.0, 20.0)]
        public decimal Percentage { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();
    }
}
