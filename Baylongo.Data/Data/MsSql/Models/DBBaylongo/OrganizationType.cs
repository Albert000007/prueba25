using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class OrganizationType
{
    public int OrganizationTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public bool IsForBaseOrganization { get; set; }

    public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>();
}
