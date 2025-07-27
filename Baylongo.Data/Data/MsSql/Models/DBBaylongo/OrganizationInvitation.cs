using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class OrganizationInvitation
{
    public Guid InvitationId { get; set; }

    public int OrganizationId { get; set; }

    public int InvitingUserId { get; set; }

    public int InvitedUserId { get; set; }

    public int RoleId { get; set; }

    public string InvitationToken { get; set; } = null!;

    public DateTime ExpirationDate { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string InvitedUserEmail { get; set; } = null!;

    public string Purpose { get; set; } = null!;

    public virtual User InvitedUser { get; set; } = null!;

    public virtual User InvitingUser { get; set; } = null!;

    public virtual Organization Organization { get; set; } = null!;

    public virtual OrganizationRole Role { get; set; } = null!;
}
