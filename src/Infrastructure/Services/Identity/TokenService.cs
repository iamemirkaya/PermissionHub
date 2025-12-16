using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Identity
{
    public class TokenService : ITokenService
    {
        

        public Task<TokenResponse> GetTokenAsync(TokenRequest tokenRequest)
        {
            throw new NotImplementedException();
        }

        public Task<TokenResponse> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            throw new NotImplementedException();
        }
    }
}
