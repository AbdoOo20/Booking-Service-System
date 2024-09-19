using System;
using System.Collections.Generic;

namespace BookingServices.Data;

public partial class BookingConsultation
{
    public int BookingId { get; set; }

    public int ConsultationId { get; set; }

    public string? Reason { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Consultation Consultation { get; set; } = null!;
}
