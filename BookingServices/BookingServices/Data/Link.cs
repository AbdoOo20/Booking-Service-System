using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Data
{
    public class Link
    {
        [ForeignKey("ServiceProvider")]
        public string? ProviderId { get; set; }

        [Required]
        public string? SocialAccount { get; set; }

        public int NumberOfClicks { get; set; }

        public virtual ServiceProvider? ServiceProvider { get; set; }
    }
}
