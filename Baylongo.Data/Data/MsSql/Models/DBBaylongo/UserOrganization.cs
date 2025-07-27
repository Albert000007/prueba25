using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class UserOrganization
{
    public int UserOrgId { get; set; }

    public int UserId { get; set; }

    public int OrganizationId { get; set; }

    public int RoleId { get; set; }

    public DateTime? JoinDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual Organization Organization { get; set; } = null!;

    public virtual OrganizationRole Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
