using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Data
{
    public class BookingService
    {
        [ForeignKey("Booking")]
        [Column(Order = 1)]
        public int BookingId { get; set; }

        [ForeignKey("Service")]
        [Column(Order = 2)]
        public int ServiceId { get; set; }

        public virtual Booking? Booking { get; set; }
        public virtual Service? Service { get; set; }
    }
}
