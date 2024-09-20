using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Data
{
    public class WishList
    {
        [ForeignKey("Customer")]
        [Column(Order = 1)]
        public string? CustomerId { get; set; }

        [ForeignKey("Service")]
        [Column(Order = 2)]
        public int ServiceId { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Service? Service { get; set; }
    }
}
