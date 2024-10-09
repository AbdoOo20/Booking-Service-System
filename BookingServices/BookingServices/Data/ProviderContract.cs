using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Data
{
    public class ProviderContract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContractId { get; set; }

        [Required]
        public string? ContractName { get; set; }

        [Required]
        public string? Details { get; set; }

        public bool? IsBlocked { get; set; } = false;

        [ForeignKey("ServiceProvider")]
        public string? ProviderId { get; set; }

        public virtual ServiceProvider? ServiceProvider { get; set; }

        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
