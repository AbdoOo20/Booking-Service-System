using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Data;

public partial class Booking
{
    [Key]
    public int BookingId { get; set; }

    public string CustomerId { get; set; }

    public int PaymentIncomeId { get; set; }

    public DateTime EventDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string Status { get; set; } = null!;

    public int? Quantity { get; set; }

    public decimal Price { get; set; }

    public int Type { get; set; }

    public DateTime BookDate { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public virtual ICollection<BookingConsultation> BookingConsultations { get; set; } = new List<BookingConsultation>();

    public virtual Customer Customer { get; set; } = null!;

    public virtual PaymentIncome PaymentIncome { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
