using ProManage360.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Application.Common.Interfaces.Service
{
    public interface IJwtTokenService
    {
        /// <summary>
        /// Generate access token (15 min expiry)
        /// </summary>
        string GenerateAccessToken(Dictionary<string, string> claims);

        /// <summary>
        /// Generate refresh token (7 days expiry)
        /// </summary>
        string GenerateRefreshToken();
    }
}
