using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingServices.Data
{
    public class ServiceImage
    {
        [ForeignKey("Service")]
        public int ServiceId { get; set; }

        [Required]
        public string? URL { get; set; }

        public virtual Service? Service { get; set; }
    }
}
