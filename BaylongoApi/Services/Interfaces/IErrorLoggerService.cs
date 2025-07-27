namespace BaylongoApi.Services.Interfaces
{
    public interface IErrorLoggerService
    {
        void LogError(string source, Exception ex, int? userId = null, string email = null,
                 object additionalData = null, string requestUrl = null,
                 string method = null, int? statusCode = null, string stripeEventId = null,
    string stripeAccountId = null,
    object eventPayload = null);
    }
}
