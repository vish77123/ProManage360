// ============================================
// Common/Behaviors/UnhandledExceptionBehavior.cs
// ============================================
namespace ProManage360.Application.Common.Behaviors;

using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Pipeline behavior that catches and logs unhandled exceptions
/// This is a safety net for unexpected errors
/// </summary>
public class UnhandledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> _logger;

    public UnhandledExceptionBehavior(ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogError(
                ex,
                "Unhandled Exception for Request {RequestName}: {@Request}",
                requestName,
                request);

            // Re-throw to let global exception handler in WebAPI catch it
            throw;
        }
    }
}