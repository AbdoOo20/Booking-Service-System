namespace CustomerProject.DTO.Review
{
    public class PostedReviewDTO
    {
        public string? CustomerId { get; set; }

        public int BookingId { get; set; }

        public decimal Rating { get; set; }

        public string? CustomerComment { get; set; }

    }
}
