using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Data
{
    public class ServicePrice
    {
        [ForeignKey("Service")]
        [ Column(Order = 1)]
        public int ServiceId { get; set; }

        [ Column(Order = 2)]
        public DateTime PriceDate { get; set; }

        [Range(1.0, double.MaxValue)]
        public decimal Price { get; set; }

        public virtual Service? Service { get; set; }
    }
}
