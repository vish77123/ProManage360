// ============================================
// Common/Behaviors/PerformanceBehavior.cs
// ============================================
namespace ProManage360.Application.Common.Behaviors;

using MediatR;
using Microsoft.Extensions.Logging;
using ProManage360.Application.Common.Interfaces;
using ProManage360.Application.Common.Interfaces.Service;
using System.Diagnostics;

/// <summary>
/// Pipeline behavior that tracks performance and warns about slow operations
/// </summary>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly Stopwatch _timer;

    // Threshold for logging slow operations (in milliseconds)
    private const int SlowOperationThresholdMs = 500;

    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
        _timer = new Stopwatch();
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        // Only log if operation took longer than threshold
        if (elapsedMilliseconds > SlowOperationThresholdMs)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId;
            var tenantId = _currentUserService.TenantId;

            _logger.LogWarning(
                "Long Running Request: {RequestName} ({ElapsedMs}ms) " +
                "by User {UserId} (Tenant: {TenantId}). " +
                "Request: {@Request}",
                requestName,
                elapsedMilliseconds,
                userId != Guid.Empty ? userId : "Anonymous",
                tenantId != Guid.Empty ? tenantId : "N/A",
                request);
        }

        return response;
    }
}