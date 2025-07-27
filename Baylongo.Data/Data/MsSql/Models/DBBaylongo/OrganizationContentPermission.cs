using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class OrganizationContentPermission
{
    public int PermissionId { get; set; }

    public int OrganizationId { get; set; }

    public int ContentTypeId { get; set; }

    public bool? IsEnabled { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ContentType ContentType { get; set; } = null!;

    public virtual Organization Organization { get; set; } = null!;
}
