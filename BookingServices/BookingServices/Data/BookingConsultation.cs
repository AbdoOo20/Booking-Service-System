using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Data
{
    public class BookingConsultation
    {
        [ForeignKey("Booking")]
        [Column(Order = 1)]
        public int BookingId { get; set; }

        [ForeignKey("Consultation")]
        [Column(Order = 2)]
        public int ConsultationId { get; set; }

        public string? Reason { get; set; }

        public virtual Booking? Booking { get; set; }
        public virtual Consultation? Consultation { get; set; }
    }
}
