using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace server.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, message) = exception switch
            {
                ArgumentNullException ane => (StatusCodes.Status400BadRequest, $"{ane.ParamName ?? "Parameter"} is required"),
                ArgumentException ae => (StatusCodes.Status400BadRequest, ae.Message),
                InvalidOperationException ioe => (StatusCodes.Status400BadRequest, ioe.Message),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
            };

            // Log the error
            _logger.LogError(
                exception,
                "Exception occurred: {Message}",
                exception.Message
            );

            // Return error response
            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(
                new ProblemDetails
                {
                    Status = statusCode,
                    Title = statusCode == 500 ? "Internal Server Error" : "Bad Request",
                    Detail = message,
                    Instance = httpContext.Request.Path
                },
                cancellationToken
            );

            return true;
        }
    }
}