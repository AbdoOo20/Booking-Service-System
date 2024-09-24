using System.ComponentModel.DataAnnotations;

namespace BookingServices.Models
{
    public class ProviderDataVM
    {
        public string? ProviderId { get; set; }

        public decimal Balance { get; set; }

        public decimal ReservedBalance { get; set; }

        public string? ServiceDetails { get; set; }

        public decimal Rate { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }
    }
}
