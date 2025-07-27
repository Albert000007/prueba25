using System.Net;
using System.Text.Json;

namespace BaylongoApi.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, _logger);
            }
        }
        private static Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger<ErrorHandlingMiddleware> logger)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = string.Empty;

            switch (ex)
            {
                case UnauthorizedAccessException:
                    code = HttpStatusCode.Unauthorized;
                    result = ex.Message;
                    break;
                case ArgumentException:
                    code = HttpStatusCode.BadRequest;
                    result = ex.Message;
                    break;
                case KeyNotFoundException:
                    code = HttpStatusCode.NotFound;
                    result = ex.Message;
                    break;
                default:
                    logger.LogError(ex, "Unhandled exception");
                    result = "An unexpected error occurred";
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            if (string.IsNullOrEmpty(result))
            {
                result = JsonSerializer.Serialize(new { error = ex.Message });
            }

            return context.Response.WriteAsync(result);
        }

    }
}
