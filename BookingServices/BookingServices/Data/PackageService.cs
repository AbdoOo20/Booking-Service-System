using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Data
{
    public class PackageService
    {
        [ForeignKey("Package")]
        [Column(Order = 1)]
        public int PackageId { get; set; }

        [ForeignKey("Service")]
        [Column(Order = 2)]
        public int ServiceId { get; set; }

        public virtual Package? Package { get; set; }

        public virtual Service? Service { get; set; }
    }
}
