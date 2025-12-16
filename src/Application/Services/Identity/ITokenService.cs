using Common.Requests.Identity;
using Common.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Identity
{
    public interface ITokenService
    {
        Task<TokenResponse> GetTokenAsync(TokenRequest tokenRequest);

        Task<TokenResponse> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
    }
}
