using MediatR;
using ProManage360.Application.Features.Auth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Application.Features.Auth.Command.Login
{
    public class LoginCommand : IRequest<LoginResponse>
    {
        /// <summary>
        /// Command to authenticate user and generate JWT tokens
        /// </summary>
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
