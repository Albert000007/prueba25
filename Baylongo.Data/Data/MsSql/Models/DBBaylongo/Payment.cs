using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class Payment
{
    public long Id { get; set; }

    public string StripePaymentIntentId { get; set; } = null!;

    public int UserId { get; set; }

    public int EventId { get; set; }

    public int OrganizationId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? ReceiptUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual Organization Organization { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
