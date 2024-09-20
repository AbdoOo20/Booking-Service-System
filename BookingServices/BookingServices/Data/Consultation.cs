using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingServices.Data
{
    public class Consultation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ConsultationId { get; set; }

        [Required]
        [StringLength(255)]
        public string? Name { get; set; }

        [Range(0.0, double.MaxValue)]
        public decimal Price { get; set; }
        
        [Required]
        public string? Description { get; set; }

        public virtual ICollection<BookingConsultation> BookingConsultations { get; set; } = new List<BookingConsultation>();
    }
}
