
using System.Net;
using System.Text.Json;
using FluentValidation;
using SQKLocalServe.Common;
using SQKLocalServe.Common.Exceptions;
using SQKLocalServe.Common.Logging;

namespace SQKLocalServe_1.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogManager _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogManager logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An unhandled exception occurred: {ex.Message}");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            ApiException apiException => new ApiResponse<object>
            {
                ResponseCode = apiException.StatusCode,
                ResponseDetailCode = apiException.DetailCode,
                Description = apiException.Message,
                Data = null
            },
            ValidationException validationException => new ApiResponse<object>
            {
                ResponseCode = (int)HttpStatusCode.BadRequest,
                ResponseDetailCode = "VALIDATION_ERROR",
                Description = "Validation failed",
                Data = validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            },
            _ => new ApiResponse<object>
            {
                ResponseCode = (int)HttpStatusCode.InternalServerError,
                ResponseDetailCode = "SERVER_ERROR",
                Description = "An unexpected error occurred",
                Data = null
            }
        };

        context.Response.StatusCode = response.ResponseCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}
       
