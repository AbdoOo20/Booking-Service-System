using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CusromerProject.DTO.Book
{
    public class Book
    {
        public int BookId { get; set; }

        [Required(ErrorMessage = "Event date is required.")]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Start Time is required.")]
        public TimeOnly StartTime { get; set; } // set start time and end time as string in API

        [Required(ErrorMessage = "End Time is required.")]
        public TimeOnly EndTime { get; set; }

        [StringLength(20), JsonIgnore]
        public string? Status { get; set; } // pending - paid - complete - canceled

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(1.0, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Payment method is required.")]
        public required string CashOrCashByHandOrInstallment { get; set; }

        public DateTime BookDate { get; set; } = DateTime.Now;

        [StringLength(20)]
        public string? Type { get; set; } //Package - Service - Consultation

        public string? CustomerId { get; set; }

        public int ServiceId { get; set; }

        public int? PaymentIncomeId { get; set; }
    }
}
