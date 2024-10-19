using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Data
{
    public class ProviderContract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContractId { get; set; }

        [Required, Display(Name = "Title"), MinLength(3, ErrorMessage = "Length must be at least 3 character")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Contract Name can only contain letters, numbers and spaces.")]
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
