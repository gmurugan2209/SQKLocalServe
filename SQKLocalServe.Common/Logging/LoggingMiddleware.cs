using Microsoft.AspNetCore.Http;

namespace SQKLocalServe.Common.Logging;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogManager _logger;

    public LoggingMiddleware(RequestDelegate next, ILogManager logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log the request
        _logger.LogInfo($"HTTP {context.Request.Method} {context.Request.Path} started");

        // Capture the start time
        var startTime = DateTime.UtcNow;

        try
        {
            // Continue processing the request
            await _next(context);

            // Calculate duration
            var duration = DateTime.UtcNow - startTime;

            // Log the response
            _logger.LogInfo($"HTTP {context.Request.Method} {context.Request.Path} - Response: {context.Response.StatusCode} - Duration: {duration.TotalMilliseconds:F2}ms");
        }
        catch (Exception ex)
        {
            // Log any uncaught exceptions
            _logger.LogError(ex, $"HTTP {context.Request.Method} {context.Request.Path} - Unhandled exception");
            throw; // Re-throw to let error handling middleware deal with it
        }
    }
}
