using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool RequiresOnlinePayment { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
