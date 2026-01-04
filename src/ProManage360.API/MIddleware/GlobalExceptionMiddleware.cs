namespace ProManage360.API.Middleware;

using System.Net;
using System.Text.Json;
using ProManage360.Application.Common.Exceptions;
using ProManage360.Domain.Exceptions;

/// <summary>
/// Global exception handler middleware
/// Catches all exceptions and converts them to proper HTTP responses
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        object response;
        int statusCode;

        // Use if-else instead of switch expression
        if (exception is ValidationException validationEx)
        {
            statusCode = (int)HttpStatusCode.BadRequest;
            response = new
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "One or more validation errors occurred.",
                Status = statusCode,
                Errors = validationEx.Errors,
                TraceId = context.TraceIdentifier
            };
        }
        else if (exception is NotFoundException notFoundEx)
        {
            statusCode = (int)HttpStatusCode.NotFound;
            response = new
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "The specified resource was not found.",
                Status = statusCode,
                Detail = notFoundEx.Message,
                TraceId = context.TraceIdentifier
            };
        }
        else if (exception is ForbiddenAccessException forbiddenEx)
        {
            statusCode = (int)HttpStatusCode.Forbidden;
            response = new
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                Title = "Forbidden",
                Status = statusCode,
                Detail = forbiddenEx.Message,
                TraceId = context.TraceIdentifier
            };
        }
        else if (exception is ConflictException conflictEx)
        {
            statusCode = (int)HttpStatusCode.Conflict;
            response = new
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                Title = "A conflict occurred.",
                Status = statusCode,
                Detail = conflictEx.Message,
                TraceId = context.TraceIdentifier
            };
        }
        else if (exception is TenantLimitExceededException limitEx)
        {
            statusCode = (int)HttpStatusCode.Forbidden;
            response = new
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                Title = "Subscription limit exceeded.",
                Status = statusCode,
                Detail = limitEx.Message,
                TraceId = context.TraceIdentifier
            };
        }
        else if (exception is BusinessRuleValidationException businessEx)
        {
            statusCode = (int)HttpStatusCode.BadRequest;
            response = new
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Business rule violation.",
                Status = statusCode,
                Detail = businessEx.Message,
                TraceId = context.TraceIdentifier
            };
        }
        else
        {
            // Default - Unexpected errors
            statusCode = (int)HttpStatusCode.InternalServerError;
            response = new
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "An error occurred while processing your request.",
                Status = statusCode,
                Detail = "An unexpected error occurred. Please try again later.",
                TraceId = context.TraceIdentifier
            };
        }

        context.Response.StatusCode = statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}

// ============================================
// Extension method for easy registration
// ============================================
public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}