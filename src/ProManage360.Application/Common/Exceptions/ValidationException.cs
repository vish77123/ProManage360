using FluentValidation.Results;

namespace ProManage360.Application.Common.Exceptions
{
    public class ValidationException : ApplicationException
    {
        /// <summary>
        /// Exception thrown when FluentValidation fails
        /// Contains all validation errors
        /// </summary>
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(
                    failureGroup => failureGroup.Key,
                    failureGroup => failureGroup.ToArray());
        }
    }
}

/*
🎯 EXAMPLE USAGE:

var validator = new RegisterTenantCommandValidator();
var result = await validator.ValidateAsync(command);

if (!result.IsValid)
{
    throw new ValidationException(result.Errors);
}

🎯 API RESPONSE (400 Bad Request):
{
  "errors": {
    "TenantName": ["Tenant name is required"],
    "Email": ["Invalid email format"],
    "Subdomain": ["Subdomain already taken"]
  }
}
*/
