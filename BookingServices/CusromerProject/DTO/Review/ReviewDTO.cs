using BookingServices.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CusromerProject.DTO.Review
{
    public class ReviewDTO
    {
        public string? CustomerId { get; set; }

        public int BookingId { get; set; }

        public decimal Rating { get; set; }

        public string? CustomerComment { get; set; }

        public DateTime CustomerCommentDate { get; set; } = DateTime.Now;

        public string? ProviderReplayComment { get; set; }

        public DateTime? ProviderReplayCommentDate { get; set; }

    }
}