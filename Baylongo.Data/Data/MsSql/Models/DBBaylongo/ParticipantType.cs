using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class ParticipantType
{
    public int ParticipantTypeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();
}
