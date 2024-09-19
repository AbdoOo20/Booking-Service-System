using System;
using System.Collections.Generic;

namespace BookingServices.Data;

public partial class UserMessage
{
    public int MsgId { get; set; }

    public int? SenderId { get; set; }

    public int? ReceiverId { get; set; }

    public string MessageText { get; set; } = null!;

    public DateTime? DateSent { get; set; }
}
