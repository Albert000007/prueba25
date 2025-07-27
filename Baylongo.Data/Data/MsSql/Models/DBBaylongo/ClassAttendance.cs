using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class ClassAttendance
{
    public int ClassAttendanceId { get; set; }

    public int EventId { get; set; }

    public int UserId { get; set; }

    public DateTime AttendanceDate { get; set; }

    public int CreditsUsed { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
