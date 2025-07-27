using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class StripeAccount
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? OrganizationId { get; set; }

    public string StripeAccountId { get; set; } = null!;

    public string AccountType { get; set; } = null!;

    public string OnboardingStatus { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdated { get; set; }

    public virtual Organization? Organization { get; set; }

    public virtual User User { get; set; } = null!;
}
