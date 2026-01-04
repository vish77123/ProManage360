namespace ProManage360.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when there's a conflict (e.g., duplicate)
/// Returns 409 Conflict
/// </summary>
public class ConflictException : ApplicationException
{
    public ConflictException(string message)
        : base(message)
    {
    }
}

/*
🎯 EXAMPLE USAGE:

var subdomainExists = await context.Tenants
    .AnyAsync(t => t.Subdomain == subdomain);
    
if (subdomainExists)
{
    throw new ConflictException($"Subdomain '{subdomain}' is already taken.");
}

🎯 API RESPONSE (409 Conflict):
{
  "message": "Subdomain 'acme' is already taken."
}
*/
