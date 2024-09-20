using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Data
{
    public class Review
    {
        [Column(Order = 1)]
        [ForeignKey("Customer")]
        public string? CustomerId { get; set; }

        [Column(Order = 2)]
        [ForeignKey("Booking")]
        public int BookingId { get; set; }

        [Range(0.0, 5.0)]
        public decimal Rating { get; set; }

        public string? CustomerComment { get; set; }

        public DateTime CustomerCommentDate { get; set; } = DateTime.Now;

        public string? ProviderReplayComment { get; set; }

        public DateTime? ProviderReplayCommentDate { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Booking? Booking { get; set; }
    }
}
