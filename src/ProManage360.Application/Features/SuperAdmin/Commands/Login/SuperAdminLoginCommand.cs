using MediatR;
using ProManage360.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Application.Features.SuperAdmin.Commands.Login
{
    public class SuperAdminLoginCommand : IRequest<Result<SuperAdminLoginResponse>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
