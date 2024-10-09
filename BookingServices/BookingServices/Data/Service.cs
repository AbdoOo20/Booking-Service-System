using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Data
{
    public class Service
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ServiceId { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        [Required]
        public string? Details { get; set; }

        [Required]
        [StringLength(100)]
        public string? Location { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0.0, 20.0)]
        public decimal InitialPaymentPercentage { get; set; }

        public bool IsOnlineOrOffline { get; set; }

        public bool IsRequestedOrNot {  get; set; }

        public bool IsBlocked { get; set; } = false;

        [ForeignKey("Category")]
        public int? CategoryId { get; set; }

        [ForeignKey("BaseService")]
        public int? BaseServiceId { get; set; }

        [ForeignKey("ProviderContract")]
        public int? ProviderContractId { get; set; }

        [ForeignKey("AdminContract")]
        public int? AdminContractId { get; set; }

        [ForeignKey("ServiceProvider")]
        public string? ProviderId { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ServiceProvider? ServiceProvider { get; set; }
        public virtual ProviderContract? ProviderContract { get; set; }
        public virtual AdminContract? AdminContract { get; set; }
        public virtual Service? BaseService { get; set; }
        
        public virtual ICollection<Service>? Relatedservices { get; set; } = new List<Service>();
        
        public virtual ICollection<ServicePrice> ServicePrices { get; set; } = new List<ServicePrice>();

        public virtual ICollection<ServiceImage> ServiceImages { get; set; } = new List<ServiceImage>();
        
        public virtual ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();

        public virtual ICollection<PackageService> PackageServices { get; set; } = new List<PackageService>();
    }
}
