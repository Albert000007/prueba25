using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class UserDancePreference
{
    public int UserId { get; set; }

    public int DanceTypeId { get; set; }

    public int PreferenceLevel { get; set; }

    public virtual DanceType DanceType { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
