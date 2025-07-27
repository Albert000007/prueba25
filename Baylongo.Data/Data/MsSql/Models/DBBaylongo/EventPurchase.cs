using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class EventPurchase
{
    public int PurchaseId { get; set; }

    public int EventId { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PurchaseDate { get; set; }

    public int StatusId { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual ICollection<PurchaseToken> PurchaseTokens { get; set; } = new List<PurchaseToken>();

    public virtual PurchaseStatus Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
