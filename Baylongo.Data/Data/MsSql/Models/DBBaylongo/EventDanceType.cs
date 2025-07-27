using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class EventDanceType
{
    public int EventId { get; set; }

    public int DanceTypeId { get; set; }

    public bool IsPrimary { get; set; }

    public virtual DanceType DanceType { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;
}
