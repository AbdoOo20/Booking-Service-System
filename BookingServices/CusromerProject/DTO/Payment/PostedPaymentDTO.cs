using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CustomerProject.DTO.Payment
{
    public class PostedPaymentDTO
    {
        public string? CustomerId { get; set; }

        public int BookingId { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Range(1.0, double.MaxValue)]
        public decimal PaymentValue { get; set; }
    }
}
