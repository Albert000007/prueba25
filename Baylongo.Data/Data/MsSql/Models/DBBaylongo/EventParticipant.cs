using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class EventParticipant
{
    public int EventParticipantId { get; set; }

    public int EventId { get; set; }

    public int ParticipantId { get; set; }

    public int RoleId { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public int UserId { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual Participant Participant { get; set; } = null!;

    public virtual ParticipantRole Role { get; set; } = null!;
}
