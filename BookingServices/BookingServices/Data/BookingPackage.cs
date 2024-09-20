using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingServices.Data
{
    public class BookingPackage
    {
        [ForeignKey("Booking")]
        [Column(Order = 1)]
        public int BookingId { get; set; }
        
        [ForeignKey("Package")]
        [Column(Order = 2)]
        public int PackageId { get; set; }

        public virtual Booking? Booking { get; set; }
        public virtual Package? Package { get; set; }
    }
}
