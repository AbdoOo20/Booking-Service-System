using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingServices.Data
{
    public class ServiceProvider
    {
        [Key]
        [ForeignKey("IdentityUser")]
        public string? ProviderId { get; set; }

        [Required]
        public required string Name { get; set; }

        [Range(0.00, double.MaxValue, ErrorMessage = "Balance must be greater than or equal zero.")]
        public decimal Balance { get; set; }

        [Range(0.00, double.MaxValue, ErrorMessage = "Reserved Balance must be greater than or equal zero.")]
        public decimal ReservedBalance { get; set; }

        public string? ServiceDetails { get; set; }

        [Range(0.0, 5.0)]
        public decimal Rate { get; set; }

        public virtual IdentityUser? IdentityUser { get; set; }
        
        public virtual ICollection<Package> Packages { get; set; } = new List<Package>();

        public virtual ICollection<ProviderContract> ProviderContracts { get; set; } = new List<ProviderContract>();

        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
