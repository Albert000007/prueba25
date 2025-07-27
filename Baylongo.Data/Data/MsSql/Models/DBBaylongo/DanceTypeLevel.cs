using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class DanceTypeLevel
{
    public int DanceTypeId { get; set; }

    public int LevelId { get; set; }

    public bool IsBeginnerFriendly { get; set; }

    public virtual DanceType DanceType { get; set; } = null!;

    public virtual DanceLevel Level { get; set; } = null!;
}
