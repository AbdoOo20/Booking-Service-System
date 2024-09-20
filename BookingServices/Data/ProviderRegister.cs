using System;
using System.Collections.Generic;

namespace BookingServices.Data;

public partial class ProviderRegister
{
    public int ProviderRegisterId { get; set; }

    public string ProviderName { get; set; } = null!;

    public string ServiceDetails { get; set; } = null!;

    public string ImgSsn { get; set; } = null!;

    public string ProviderLocation { get; set; } = null!;

    public string ProviderPhoneNumber { get; set; } = null!;
}
