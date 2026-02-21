using Application.Services.Identity;
using Common.Responses.Wrappers;
using Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Token.Queries
{
    public class VerifyTwoFactorQuery : IRequest<ResponseWrapper<TokenResponse>>
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }

    public class VerifyTwoFactorQueryHandler : IRequestHandler<VerifyTwoFactorQuery, ResponseWrapper<TokenResponse>>
    {
        private readonly ITokenService _tokenService;
        public VerifyTwoFactorQueryHandler(ITokenService tokenService) => _tokenService = tokenService;

        public async Task<ResponseWrapper<TokenResponse>> Handle(VerifyTwoFactorQuery request, CancellationToken ct)
            => await _tokenService.VerifyTwoFactorAsync(request.Email, request.Code);
    }
}
