using System;
using System.Collections.Generic;

namespace BookingServices.Data;

public partial class UserMessage
{
    public int MsgId { get; set; }

    public string? SenderId { get; set; }

    public string? ReceiverId { get; set; }

    public string MessageText { get; set; } = null!;

    public DateTime? DateSent { get; set; }
}
