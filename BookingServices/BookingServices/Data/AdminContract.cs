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

        [Required, Display(Name = "Title"), MinLength(10, ErrorMessage = "Length must be at least 10 character")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Title can only contain letters and spaces.")]
        public string? ContractName { get; set; }

        [Required, Display(Name = "Details"), MinLength(20, ErrorMessage = "Length must be at least 20 character")]
        public string? Details { get; set; }

        public  bool? IsBlocked { get; set; } = false;

        [ForeignKey("IdentityUser")]
        public string? UserId { get; set; }
        public virtual IdentityUser? IdentityUser { get; set; }

        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
