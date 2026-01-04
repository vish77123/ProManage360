using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Application.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}
