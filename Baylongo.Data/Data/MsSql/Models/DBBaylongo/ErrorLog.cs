using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class ErrorLog
{
    public int ErrorLogId { get; set; }

    public DateTime ErrorTime { get; set; }

    public string ErrorLevel { get; set; } = null!;

    public string Source { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? ExceptionType { get; set; }

    public string? StackTrace { get; set; }

    public int? UserId { get; set; }

    public string? Email { get; set; }

    public string? AdditionalData { get; set; }

    public int? StatusCode { get; set; }

    public string? RequestUrl { get; set; }

    public string? Method { get; set; }

    public string? CreatedBy { get; set; }

    public string? MachineName { get; set; }

    public string? StripeEventId { get; set; }

    public string? StripeAccountId { get; set; }

    public string? EventPayload { get; set; }

    public virtual User? User { get; set; }
}
