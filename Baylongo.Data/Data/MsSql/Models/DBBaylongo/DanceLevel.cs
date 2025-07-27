using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class DanceLevel
{
    public int LevelId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int Order { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<DanceTypeLevel> DanceTypeLevels { get; set; } = new List<DanceTypeLevel>();
}
