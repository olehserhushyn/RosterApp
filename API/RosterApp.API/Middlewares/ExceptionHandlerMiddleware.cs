using System.Net;
using System.Text.Json;

namespace RosterApp.API.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string message = "An unexpected error occurred. Please try again later.";

            switch (exception)
            {
                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;
                case ArgumentException:
                case InvalidOperationException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;
                // 401/403
                default:
                    // 50x
                    _logger.LogError(exception, "A critical unhandled exception occurred.");
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var errorDetails = new
            {
                StatusCode = context.Response.StatusCode,
                Message = message,
                Detail = statusCode == HttpStatusCode.InternalServerError 
                    && context.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment()
                    ? exception.ToString()
                    : null
            };

            var json = JsonSerializer.Serialize(errorDetails);
            return context.Response.WriteAsync(json);
        }
    }
}
