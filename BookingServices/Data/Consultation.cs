using System;
using System.Collections.Generic;

namespace BookingServices.Data;

public partial class Consultation
{
    public int ConsultationId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<BookingConsultation> BookingConsultations { get; set; } = new List<BookingConsultation>();
}
