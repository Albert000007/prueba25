using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class Participant
{
    public int ParticipantId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int ParticipantTypeId { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Website { get; set; }

    public string? SocialMedia { get; set; }

    public string? LogoUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();

    public virtual ParticipantType ParticipantType { get; set; } = null!;
}
