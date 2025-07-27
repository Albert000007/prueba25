using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class OrganizationRole
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public bool? CanManageOrganization { get; set; }

    public bool? CanCreateEvents { get; set; }

    public bool? CanInviteMembers { get; set; }

    public bool? CanCurses { get; set; }

    public bool? CanWorkshops { get; set; }

    public virtual ICollection<OrganizationInvitation> OrganizationInvitations { get; set; } = new List<OrganizationInvitation>();

    public virtual ICollection<UserOrganization> UserOrganizations { get; set; } = new List<UserOrganization>();
}
