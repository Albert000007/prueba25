using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class DanceType
{
    public int DanceTypeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public string? IconUrl { get; set; }

    public int? CategoryId { get; set; }

    public int Order { get; set; }

    public virtual DanceCategory? Category { get; set; }

    public virtual ICollection<DanceTypeLevel> DanceTypeLevels { get; set; } = new List<DanceTypeLevel>();

    public virtual ICollection<EventDanceType> EventDanceTypes { get; set; } = new List<EventDanceType>();

    public virtual ICollection<UserDancePreference> UserDancePreferences { get; set; } = new List<UserDancePreference>();
}
