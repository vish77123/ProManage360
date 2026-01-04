namespace ProManage360.Domain.Exceptions;

/// <summary>
/// Base domain exception
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }

    protected DomainException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Exception for invalid entity state
/// </summary>
public class InvalidEntityStateException : DomainException
{
    public InvalidEntityStateException(string message) : base(message) { }
}

/// <summary>
/// Exception for business rule violations
/// </summary>
public class BusinessRuleValidationException : DomainException
{
    public BusinessRuleValidationException(string message) : base(message) { }
}

/// <summary>
/// Exception for tenant limit exceeded
/// </summary>
public class TenantLimitExceededException : DomainException
{
    public TenantLimitExceededException(string resource, int current, int max)
        : base($"Tenant limit exceeded for {resource}. Current: {current}, Max: {max}. Please upgrade your subscription.") { }
}