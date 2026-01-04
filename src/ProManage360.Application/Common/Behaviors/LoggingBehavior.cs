// ============================================
// Common/Behaviors/LoggingBehavior.cs
// ============================================
namespace ProManage360.Application.Common.Behaviors;

using MediatR;
using Microsoft.Extensions.Logging;
using ProManage360.Application.Common.Interfaces;
using ProManage360.Application.Common.Interfaces.Service;
using System.Diagnostics;

/// <summary>
/// Pipeline behavior that logs all requests and their execution time
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService.UserId;
        var tenantId = _currentUserService.TenantId;

        // Log request start
        _logger.LogInformation(
            "Handling {RequestName} by User {UserId} (Tenant: {TenantId})",
            requestName,
            userId != Guid.Empty ? userId : "Anonymous",
            tenantId != Guid.Empty ? tenantId : "N/A");

        // Execute handler and measure time
        var stopwatch = Stopwatch.StartNew();
        TResponse response;

        try
        {
            response = await next();
            stopwatch.Stop();

            // Log success
            _logger.LogInformation(
                "Handled {RequestName} successfully in {ElapsedMs}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // Log failure
            _logger.LogError(
                ex,
                "Error handling {RequestName} after {ElapsedMs}ms: {ErrorMessage}",
                requestName,
                stopwatch.ElapsedMilliseconds,
                ex.Message);

            throw; // Re-throw to let global exception handler catch it
        }

        return response;
    }
}