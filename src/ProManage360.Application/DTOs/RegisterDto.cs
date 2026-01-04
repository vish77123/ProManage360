using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Application.DTOs
{
    public class RegisterDto
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required Guid TenantId { get; set; }
    }
}
