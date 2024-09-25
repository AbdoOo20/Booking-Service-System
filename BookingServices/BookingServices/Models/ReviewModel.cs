namespace BookingServices.Models
{
    public class ReviewModel
    {
        public int BookingId { get; set; }
        public string CustomerComment { get; set; }
        public DateTime CustomerCommentDate { get; set; }
        public string CustomerId { get; set; }
        public string ProviderReplayComment { get; set; }
        public DateTime? ProviderReplayCommentDate { get; set; }

        public string ReviewerName { get; set; }
    }
}
