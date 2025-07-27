using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class DanceCategory
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int Order { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<DanceType> DanceTypes { get; set; } = new List<DanceType>();
}
