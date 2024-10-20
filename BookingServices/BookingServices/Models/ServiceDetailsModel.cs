using BookingServices.Data;

namespace BookingServices.Models
{
    public class ServiceDetailsModel
    {
        public int? ServiceId { get; set; }

        public string ServiceName { get; set; }

        public decimal servicePrice { get; set; }

        public TimeSpan startTime { get; set; }
        public TimeSpan endTime { get; set; }
        public int AvailableQuantity { get; set; }

        public string ServiceDetails { get; set; }

        public string serviceLocation { get; set; }

        public string providerName { get; set; }

        public string ReviewerName { get; set; }

        public string CategoryName { get; set; }

        public List<string> serviceImages { get; set; } = new List<string>();

        public List<ReviewModel> Reviews { get; set; } = new List<ReviewModel>();

        public decimal providerRate { get; set; }

        public int numberOfReviews { get; set; }

        public string? customerID { get; set; }

        public string ProviderID { get; set; }



    }
}
