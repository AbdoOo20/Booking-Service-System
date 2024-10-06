using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Data
{
    public class Package
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PackageId { get; set; }

        [Required]
        [StringLength(255)]
        public string? PackageName { get; set; }

        [Range(1.0, double.MaxValue)]
        public decimal Price { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string? PackageStatus { get; set; }

        [Range(0.0, 20.0)]
        public decimal PercentageForAdmin { get; set; }

        public bool? IsBlocked { get; set; }

        [ForeignKey("ServiceProvider")]
        public string? ProviderId { get; set; }

        public virtual ServiceProvider? ServiceProvider { get; set; }

        public virtual ICollection<BookingPackage> BookingPackages { get; set; } = new List<BookingPackage>();

        public virtual ICollection<PackageService> PackageServices { get; set; } = new List<PackageService>();
    }
}
