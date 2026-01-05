using MediatR;
using ProManage360.Application.Features.Auth.DTOs;

namespace ProManage360.Application.Features.Auth.Command.RefreshToken
{
    public class RefreshTokenCommand : IRequest<RefreshTokenResponse>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
