using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class StripeEvent
{
    public int Id { get; set; }

    public string StripeEventId { get; set; } = null!;

    public string EventType { get; set; } = null!;

    public string Payload { get; set; } = null!;

    public bool Processed { get; set; }

    public DateTime CreatedAt { get; set; }
}
