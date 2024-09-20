using System;
using System.Collections.Generic;

namespace BookingServices.Data;

public partial class Link
{
    public string ProviderId { get; set; } = null!;

    public string SocialAccount { get; set; } = null!;

    public int? NumberOfClicks { get; set; }

    public virtual ServiceProvider Provider { get; set; } = null!;
}
