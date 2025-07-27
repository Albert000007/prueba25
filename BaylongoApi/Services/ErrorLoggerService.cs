using Baylongo.Data.Data.MsSql.Contexts.SqlServer.BaylongoContext;
using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using BaylongoApi.Services.Interfaces;
using Stripe;
using System.Text.Json;

namespace BaylongoApi.Services
{
    public class ErrorLoggerService(BaylongoContext context, ILogger<ErrorLoggerService> logger) : IErrorLoggerService
    {
        public void LogError(
    string source,
    Exception ex,
    int? userId = null,
    string email = null,
    object additionalData = null,
    string requestUrl = null,
    string method = null,
    int? statusCode = null,
    // Nuevos parámetros para Stripe
    string stripeEventId = null,
    string stripeAccountId = null,
    object eventPayload = null)
        {
            try
            {
                var errorLog = new ErrorLog
                {
                    ErrorTime = DateTime.UtcNow, // Asegurar fecha incluso si no está en el modelo
                    Source = source,
                    ErrorLevel = "Error",
                    Message = ex.Message.Length > 4000 ? ex.Message[..4000] : ex.Message, // Truncar si es necesario
                    ExceptionType = ex.GetType().Name,
                    StackTrace = ex.StackTrace,
                    UserId = userId,
                    Email = email,
                    AdditionalData = additionalData != null ?
                        JsonSerializer.Serialize(additionalData, new JsonSerializerOptions { WriteIndented = false }) :
                        null,
                    RequestUrl = requestUrl,
                    Method = method,
                    StatusCode = statusCode,
                    CreatedBy = "System",
                    MachineName = Environment.MachineName,
                    // Nuevos campos para Stripe
                    StripeEventId = stripeEventId,
                    StripeAccountId = stripeAccountId,
                    EventPayload = eventPayload != null ?
                        JsonSerializer.Serialize(eventPayload) :
                        null
                };

                context.ErrorLogs.Add(errorLog);
                context.SaveChanges();

                logger.LogError(ex,
                    "Error registrado en DB. " +
                    "StripeEventId: {StripeEventId}, " +
                    "StripeAccountId: {StripeAccountId}",
                    stripeEventId, stripeAccountId);
            }
            catch (Exception logEx)
            {
                logger.LogCritical(logEx,
                    "Fallo al registrar error en base de datos. " +
                    "Error original: {OriginalMessage}",
                    ex.Message);

                // Opcional: Notificación inmediata para errores críticos
                if (ex is StripeException stripeEx)
                {
                    //SendAlertToAdmin($"Stripe Critical Error: {stripeEx.StripeError?.Code}");
                }
            }
        }

    }
}
