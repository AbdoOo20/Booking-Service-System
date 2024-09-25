using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Data
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingId { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        [StringLength(20)]
        public string? Status { get; set; } // pending - paid - complete - canceled

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(1.0, double.MaxValue)]
        public decimal Price { get; set; }

        public bool CashOrInstallment { get; set; }

        public DateTime BookDate { get; set; } = DateTime.Now;

        [StringLength(20)]
        public string? Type { get; set; } //Package - Service - Consultation
        
        [ForeignKey("Customer")]
        public string? CustomerId { get; set; }

        [ForeignKey("PaymentIncome")]
        public int? PaymentIncomeId { get; set; }
        
        public virtual Customer? Customer { get; set; }
        public virtual PaymentIncome? PaymentIncome { get; set; }

        public virtual ICollection<BookingConsultation> BookingConsultations { get; set; } = new List<BookingConsultation>();

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        public virtual ICollection<BookingPackage> BookingPackages { get; set; } = new List<BookingPackage>();

        public virtual ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
    }
}
