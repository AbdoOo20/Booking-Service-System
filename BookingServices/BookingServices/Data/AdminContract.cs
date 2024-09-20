using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingServices.Data
{
    public class AdminContract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContractId { get; set; }
        
        [Required]
        public string? Details { get; set; }
        
        [ForeignKey("IdentityUser")]
        public string? UserId { get; set; }
        public virtual IdentityUser? IdentityUser { get; set; }

        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
