
// ============================================
// Common/Behaviors/ValidationBehavior.cs
// ============================================
namespace ProManage360.Application.Common.Behaviors;

using FluentValidation;
using MediatR;
using ProManage360.Application.Common.Exceptions;
using ValidationException = Exceptions.ValidationException;

/// <summary>
/// Pipeline behavior that automatically validates all requests
/// using FluentValidation validators
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // If no validators registered for this request type, skip validation
        if (!_validators.Any())
        {
            return await next();
        }

        // Create validation context
        var context = new ValidationContext<TRequest>(request);

        // Run all validators in parallel
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Collect all validation failures
        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToList();

        // If validation failed, throw ValidationException
        if (failures.Any())
        {
            throw new ValidationException(failures);
        }

        // Validation passed, continue to next behavior or handler
        return await next();
    }
}