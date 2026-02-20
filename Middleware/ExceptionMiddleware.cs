using System;
using System.Net;
using System.Text.Json;

namespace Timesheet.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {

                var correlationId = Guid.NewGuid().ToString();

                context.Items["CorrelationId"] = correlationId;
                context.Response.Headers.Add("X-Correlation-ID", correlationId);

                _logger.LogInformation("exception CorrelationId is {CorrelationId}", correlationId);
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var statusCode = ex switch
            {
                KeyNotFoundException => HttpStatusCode.NotFound,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                ArgumentException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                success = false,
                statusCode = context.Response.StatusCode,
                message = ex.Message
            };

            var jsonResponse = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(jsonResponse);
        }
    }

}
