using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Application.Common.Exceptions
{
    public class ApplicationException : Exception
    {
        public ApplicationException(string message) : base(message) { }

        public ApplicationException(string message, Exception innerException)
                    :base(message, innerException) { }
    }
}


// ============================================
// HTTP STATUS CODE MAPPING
// ============================================
/*
Exception Type                    HTTP Status Code
--------------------------------------------------
ValidationException               400 Bad Request
NotFoundException                 404 Not Found
ForbiddenAccessException          403 Forbidden
ConflictException                 409 Conflict
UnauthorizedException             401 Unauthorized
TenantLimitExceededException      402 Payment Required / 403 Forbidden
ApplicationException (base)       500 Internal Server Error

🎯 These will be handled by global exception middleware in WebAPI layer
*/