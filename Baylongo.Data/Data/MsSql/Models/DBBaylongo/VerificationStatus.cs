using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class VerificationStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>();
}
