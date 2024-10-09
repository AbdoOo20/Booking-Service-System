using CustomerProject.DTO.Services;

namespace CusromerProject.DTO.Services
{
    public class ServiceDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public string Location { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Quantity { get; set; }
        public decimal InitialPayment { get; set; }
        public string Category { get; set; }
        public string ProviderName { get; set; }
        public decimal PriceForTheCurrentDay { get; set; }

        public List<string> Images { get; set; }
        public List<AllServicesDetailsDTO> RelatedServices { get; set; }

        public Contract? _AdminContract { get; set; }
        public Contract? _ProviderContract { get; set; }
    }
}