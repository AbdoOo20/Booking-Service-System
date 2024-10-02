using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Models
{
    public class ServicePriceModel
    {
        public int ServiceId { get; set; }
        [Display(Name ="From Date")]
        public DateTime StartDate { get; set; }
        [Display(Name ="To Date")]
        public DateTime EndDate { get; set; }
        [Display(Name ="Date")]
        public DateTime PriceDate { get; set; }

        [Range(1.0, double.MaxValue)]
        public decimal Price { get; set; }
    }
}
