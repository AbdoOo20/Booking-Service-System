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

        [Required]
        [MinLength(10, ErrorMessage = "Name Lenght Must be at Least 10")]
        public string? Name { get; set; }

        public virtual ICollection<Service> Services { get; set; } = new List<Service>();


    }
}
