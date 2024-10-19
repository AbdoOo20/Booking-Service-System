using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingServices.Data
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        [Required, Display(Name = "Name"), MinLength(5, ErrorMessage = "Length must be at least 5 character")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Category can only contain letters and spaces.")]
        public string? Name { get; set; }

        public virtual ICollection<Service> Services { get; set; } = new List<Service>();


    }
}
