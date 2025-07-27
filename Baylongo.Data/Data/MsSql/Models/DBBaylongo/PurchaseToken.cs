using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class PurchaseToken
{
    public int PurchaseTokenId { get; set; }

    public int EventPurchaseId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public virtual EventPurchase EventPurchase { get; set; } = null!;
}
