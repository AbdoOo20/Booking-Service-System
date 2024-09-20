using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Data
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }

        [ForeignKey("Customer")]
        public string? CustomerId { get; set; }

        [ForeignKey("Booking")]
        public int BookingId { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Range(1.0, double.MaxValue)]
        public decimal PaymentValue { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Booking? Booking { get; set; }
    }
}
